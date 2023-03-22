using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gdev
{
    public abstract class AnimatorManager : MonoBehaviour
    {
        public Animator baseAnimator;

        protected virtual void Start()
        {
            baseAnimator = GetComponent<Animator>();
        }
        public abstract void UpdateAnimatorValue(float value);
        public abstract void PlayTargetAnimation(string animationName);
        public abstract void SetBoolState(string stateName, bool stateValue);
    }
}
