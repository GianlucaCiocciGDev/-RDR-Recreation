using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gdev
{
    public abstract class LocomotionManager : PlayerManager
    {
        protected abstract void HandleLocomotion();
        public abstract void HandleRotation(Quaternion current, Quaternion target);
        public abstract void HandleAimRotation(Quaternion current, Quaternion target);
        protected abstract void HandleGravity();
        protected abstract void HandleCamera();
    }
}
