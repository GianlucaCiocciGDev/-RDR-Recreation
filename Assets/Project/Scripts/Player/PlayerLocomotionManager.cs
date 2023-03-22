using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

namespace Gdev
{
    public class PlayerLocomotionManager : LocomotionManager
    {
        PlayerAnimatorManager playerAnimatorManager;
        CharacterController characterController;

        [Header("Movement Settings")]
        [SerializeField] float walkSpeed = 2.0f;
        [SerializeField] float sprintSpeed = 4.0f;
        [SerializeField] float rotationSpeed;
        private float desideredRotation;

        [Header("Gravity Settings")]
        public float verticalSpeed;
        [SerializeField] float Gravity = -15.0f;

        [Header("Cinemachine Settings")]
        [SerializeField] GameObject CinemachineCameraTarget;
        [SerializeField] float TopClamp = 70.0f;
        [SerializeField] float BottomClamp = -30.0f;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;


        public bool canRotate = true;
        public bool canMove = true;
        void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            HandleLocomotion();
            HandleGravity();
        }
        private void LateUpdate()
        {
            HandleCamera();
        }

        #region Implement Class
        protected override void HandleLocomotion()
        {
            if (!canMove)
                return;

            Vector3 movement = new Vector3(inputManager.movementAxis.x, 0, inputManager.movementAxis.y).normalized;

            Vector3 movementWithRotation = Quaternion.Euler(0, _cinemachineTargetYaw, 0) * movement;

            float nextSpeed = inputManager.holdingRun ? sprintSpeed : walkSpeed;
            Vector3 verticalmovement = new Vector3(0.0f, verticalSpeed, 0.0f);
            characterController.Move((verticalmovement + (movementWithRotation * nextSpeed)) * Time.deltaTime);

            if (canRotate)
            {
                if (movementWithRotation.magnitude > 0)
                {
                    desideredRotation = Mathf.Atan2(movementWithRotation.x, movementWithRotation.z) * Mathf.Rad2Deg;
                }
                else
                    nextSpeed = 0;
                Quaternion currentRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.Euler(0, desideredRotation, 0);
                HandleRotation(currentRotation, targetRotation);
            }
            playerAnimatorManager.UpdateAnimatorValue(Mathf.Lerp(playerAnimatorManager.GetAnimatorSpeedValue(), nextSpeed, 6 * Time.deltaTime));
        }
        public override void HandleRotation(Quaternion current, Quaternion target)
        {
            transform.rotation = Quaternion.Lerp(current, target, rotationSpeed * Time.deltaTime);
        }
        public override void HandleAimRotation(Quaternion current, Quaternion target)
        {
            transform.rotation = Quaternion.Lerp(current, target, rotationSpeed * Time.deltaTime);
            desideredRotation = target.eulerAngles.y;
        }
        protected override void HandleCamera()
        {
            if (inputManager.lookAxis.sqrMagnitude >= 0f)
            {
                _cinemachineTargetYaw += inputManager.lookAxis.x * 1.0f;
                _cinemachineTargetPitch += inputManager.lookAxis.y * 1.0f;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,
                _cinemachineTargetYaw, 0.0f);
        }
        protected override void HandleGravity()
        {
            if (Physics.Raycast(transform.position, Vector3.down, .5f, LayerMask.GetMask("Default")))
            {
                verticalSpeed = 0.01f;
            }
            else
            {
                verticalSpeed += Gravity * Time.deltaTime;
            }
        }
        #endregion

        #region private Methods
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        #endregion
    }
}
