using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gdev
{
    public abstract class ProfileManager : MonoBehaviour
    {
        protected abstract void SetColorAdjustments(State state);
        protected abstract void SetVignette(State state);
        protected abstract void SetAberration(State state);

        public void SetProfile(State type)
        {
            SetColorAdjustments(type);
            SetVignette(type);
            SetAberration(type);
        }
        public void ChangeTime(State type)
        {
            float targetValue = type == State.Aim ? .1f : 1f;
            DOVirtual.Float(Time.timeScale, targetValue, .2f, SetTimeScale);
        }
        private void SetTimeScale(float time)
        {
            Time.timeScale = time;
        }

    }
}
