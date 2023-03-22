using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gdev
{
    public class PlayerAnimatorManager : AnimatorManager
    {
        private int speedID;

        protected override void Start()
        {
            base.Start();
            speedID = Animator.StringToHash("speed");
        }
        public override void UpdateAnimatorValue(float value)
        {
            baseAnimator.SetFloat(speedID, value);
        }
        public override void PlayTargetAnimation(string animationName)
        {
            baseAnimator.CrossFade(animationName, .1f);
        }
        public float GetAnimatorSpeedValue()
        {
            return baseAnimator.GetFloat(speedID);
        }
        public override void SetBoolState(string stateName,bool stateValue)
        {
            baseAnimator.SetBool(stateName, stateValue);
        }

    }
}
