using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gdev
{
    public abstract class BotManagerBase : MonoBehaviour
    {
        protected Animator baseAnimator;
        protected bool isDied = false;
        protected virtual void Awake()
        {
            baseAnimator= GetComponent<Animator>();
        }

        protected abstract void DeatcivateRadgoll();
        public abstract void ActivateRagdoll(Transform target, Transform point);
    }
}
