using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Gdev
{
    public enum State
    {
        Normal,
        Aim
    }
    public class PlayerCombatManager : CombatManager
    {
        [SerializeField] Rig rigAimLayer;
        float rigAimWeight = 0;
        [SerializeField] Rig rigNormalLayer;
        float rigNormalWeight = 1;

        PlayerAnimatorManager playerAnimatorManager;
        PlayerLocomotionManager playerLocomotionController;
        PlayerProfileManager effectManager;

        [SerializeField] Transform aimTarget;

        Camera mainCamera;
        [SerializeField] CinemachineVirtualCamera mainPlayerCamera;
        [SerializeField] CinemachineVirtualCamera aimPlayerCamera;
        private CinemachineImpulseSource impulse;

        [Header("Reticle")]
        [SerializeField] Image reticle;

        [Header("Draw targets Setting")]
        [SerializeField] GameObject targetDrawModel;
        [SerializeField] Transform mainCanvas;

        [Header("Effects")]
        [SerializeField] ParticleSystem[] muzzles;
        [SerializeField] GameObject bloodFX;

        public List<Transform> targets = new List<Transform>();
        public List<Transform> targetsDraw = new List<Transform>();

        protected override void Awake()
        {
            base.Awake();
            mainCamera = Camera.main;
        }
        private void Start()
        {
            effectManager = GetComponent<PlayerProfileManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerLocomotionController = GetComponent<PlayerLocomotionManager>();

            impulse = aimPlayerCamera.GetComponent<CinemachineImpulseSource>();
            CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
        }
        private void Update()
        {
            UpdateTargetsDrawPostionInCanvas();
            if (inputManager.holdingAim)
            {
                HandleCombat();
            }

            rigAimLayer.weight = Mathf.Lerp(rigAimLayer.weight, rigAimWeight, Time.deltaTime * 20f);
            rigNormalLayer.weight = Mathf.Lerp(rigNormalLayer.weight, rigNormalWeight, Time.deltaTime * 20f);

        }

        protected override void HandleCombat()
        {
            Vector3 mouseWorldPosition = Vector3.zero;
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = mainCamera.ScreenPointToRay(screenCenterPoint);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue))
            {
                if (hit.transform.root.gameObject == this.transform.gameObject)
                    return;

                mouseWorldPosition = hit.point;
                if (hit.transform?.gameObject?.layer == 10)
                {
                    ChangeReticleColor(Color.red);
                    if (!targets.Contains(hit.transform) && !hit.transform.GetComponentInParent<BotManager>().IsDied())
                    {
                        targets.Add(hit.transform);

                        Vector3 convertedPos = Camera.main.WorldToScreenPoint(hit.transform.position);
                        GameObject targetsX = Instantiate(targetDrawModel, convertedPos, Quaternion.identity, mainCanvas);
                        targetsDraw.Add(targetsX.transform);
                    }
                }
                ChangeReticleColor(Color.white);
            }
            else
            {
                mouseWorldPosition = ray.origin + ray.direction * 900f;
            }

            aimTarget.position = mouseWorldPosition;

            RotateToAimDirection();
        }
        public override void HandleAiming(State currentStatestate) => StateChange(currentStatestate);
        private void RotateToAimDirection()
        {
            Vector3 aimDirection = (aimTarget.position - transform.position).normalized;
            aimDirection.y = 0.0f;
            playerLocomotionController.HandleAimRotation(transform.rotation, Quaternion.LookRotation(aimDirection));
        }
        private void ExecShoot()
        {
            if (targets.Count > 0)
            {
                Sequence s = DOTween.Sequence();
                for (int i = 0; i < targets.Count; i++)
                {
                    int x = i;
                    s.Append(aimTarget.transform.DOMove(targets[x].position, 0.5f).SetUpdate(true));
                    s.AppendCallback(() => PlayParticle()).SetUpdate(true);
                    s.Append(targetsDraw[x].GetComponent<Image>().DOColor(Color.clear, .1f).SetUpdate(true));
                    s.AppendCallback(() => Instantiate(bloodFX, targets[x]).transform.DOScale(new Vector3(.8f, .8f, .8f), .01f).SetUpdate(true));
                    s.AppendCallback(() => targets[x].GetComponentInParent<BotManager>().ActivateRagdoll(transform, targets[x].transform));
                    s.AppendInterval(.5f);
                }
                s.AppendCallback(() => ClearTargets());
                s.AppendCallback(() => StateChange(State.Normal));
                s.AppendInterval(.5f);
                s.AppendCallback(() => playerAnimatorManager.PlayTargetAnimation("Reload"));

            }
        }
        private void UpdateTargetsDrawPostionInCanvas()
        {
            if (targets.Count > 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    targetsDraw[i].position = Camera.main.WorldToScreenPoint(targets[i].position);
                }
            }
        }
        private void ClearTargets()
        {
            targets.Clear();

            foreach (Transform t in targetsDraw)
            {
                Destroy(t.gameObject);
            }
            targetsDraw.Clear();
        }
        private void PlayParticle()
        {
            if (impulse)
                impulse.GenerateImpulse();

            foreach (ParticleSystem muzzle in muzzles)
            {
                muzzle.Emit(1);
            }
        }
        private void SetLayerWeight(State type)
        {
            rigAimWeight = type == State.Aim ? 1.0f : 0.0f;
            rigNormalWeight = type == State.Normal ? 1.0f : 0.0f;
        }
        protected override void StateChange(State state)
        {
            if (state == State.Normal)
            {
                if (targets.Count > 0)
                {
                    effectManager.ChangeTime(State.Aim);
                    ExecShoot();
                }
                else
                {
                    effectManager.ChangeTime(State.Normal);
                    reticle.transform.DOScale(state == State.Aim ? 1.0f : 0.0f, .1f);
                    aimPlayerCamera.gameObject.SetActive(state == State.Aim);
                    SetLayerWeight(state);
                    playerAnimatorManager.SetBoolState("IsAiming", state == State.Aim);
                    effectManager.SetProfile(state);
                    playerLocomotionController.canRotate = playerLocomotionController.canMove = state == State.Normal;
                }
            }
            else
            {
                reticle.transform.DOScale(state == State.Aim ? 1.0f : 0.0f, .1f);
                aimPlayerCamera.gameObject.SetActive(state == State.Aim);
                SetLayerWeight(state);
                playerAnimatorManager.SetBoolState("IsAiming", state == State.Aim);
                effectManager.SetProfile(state);
                playerLocomotionController.canRotate = playerLocomotionController.canMove = state == State.Normal;
            }
        }
        private void ChangeReticleColor(Color color)
        {
            reticle.DOColor(color, .2f);
        }
    }
}
