using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Windows;

namespace Gdev
{
    public class InputManager : MonoBehaviour
    {
        MapInput inputMap;

        public Vector2 movementAxis;
        public Vector2 lookAxis;

        public bool holdingRun;
        public bool holdingAim;
        public bool fireTrigger;

        PlayerCombatManager playerCombatManager;


        private void Awake()
        {
            inputMap = new MapInput();

            inputMap.Player.Quit.started += OnQuitGame;

            inputMap.Player.Look.started += OnLook;
            inputMap.Player.Look.performed += OnLook;
            inputMap.Player.Look.canceled += OnLook;

            inputMap.Player.Movement.started += OnMove;
            inputMap.Player.Movement.performed += OnMove;
            inputMap.Player.Movement.canceled += OnMove;

            inputMap.Player.Run.started += OnRun;
            inputMap.Player.Movement.performed += OnRun;
            inputMap.Player.Run.canceled += OnRun;

            inputMap.Player.Aim.started += OnAim;
            inputMap.Player.Aim.canceled += OnAim;

            inputMap.Player.Fire.started += OnShoot;
            inputMap.Player.Fire.canceled += OnShoot;
            ToggleMouseCursor(false);

            //InputSystem.onActionChange += (obj, change) =>
            //{
            //    print(obj);
            //    print(change);
            //};
        }
        private void Start()
        {
            playerCombatManager= GetComponent<PlayerCombatManager>();
        }

        #region Custom Input Events
        private void OnMove(InputAction.CallbackContext context)
        {
            movementAxis = context.ReadValue<Vector2>();
        }
        public void OnLook(InputAction.CallbackContext context)
        {
            lookAxis = context.ReadValue<Vector2>();
        }
        public void OnShoot(InputAction.CallbackContext context)
        {
            switch (context)
            {
                case { phase: InputActionPhase.Started }:
                    fireTrigger = true;
                    break;
                case { phase: InputActionPhase.Performed }:
                    break;

                case { phase: InputActionPhase.Canceled }:
                    fireTrigger = false;
                    break;
            }
        }
        public void OnAim(InputAction.CallbackContext context)
        {
            
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    holdingAim = true;
                    playerCombatManager.HandleAiming(State.Aim);
                    break;
                case InputActionPhase.Canceled:
                    playerCombatManager.HandleAiming(State.Normal);
                    holdingAim = false;
                    break;
            }
        }
        public void OnRun(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    holdingRun = true;
                    break;
                case InputActionPhase.Canceled:
                    holdingRun = false;
                    break;
            }
        }
        public void OnQuitGame(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Application.Quit();
                    break;
            }
        }
        #endregion

        #region Utility Methods
        private void ToggleMouseCursor(bool value)
        {
            Cursor.visible= value;
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        }

        #endregion

        private void OnEnable()
        {
            inputMap.Enable();
        }
        private void OnDisable()
        {
            inputMap.Enable();
        }

    }
}

