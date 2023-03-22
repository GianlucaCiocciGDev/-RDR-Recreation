using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gdev
{
    public class BotManager : BotManagerBase
    {
        Rigidbody[] rigidbodies;
        void Start()
        {
            rigidbodies = GetComponentsInChildren<Rigidbody>();
            DeatcivateRadgoll();
        }
        protected override void DeatcivateRadgoll()
        {
            base.isDied = false;
            baseAnimator.enabled = true;
            foreach (Rigidbody currentRigidbody in rigidbodies)
            {
                currentRigidbody.isKinematic = true;
            }
        }
        public override void ActivateRagdoll(Transform target, Transform point)
        {
            baseAnimator.enabled = false;
            foreach (Rigidbody currentRigidbody in rigidbodies)
            {
                currentRigidbody.isKinematic = false;
            }
            point.GetComponent<Rigidbody>().AddForce(target.forward * 30, ForceMode.Impulse);
            base.isDied = true;
        }
        public bool IsDied()
        {
            return base.isDied;
        }
    }
}
