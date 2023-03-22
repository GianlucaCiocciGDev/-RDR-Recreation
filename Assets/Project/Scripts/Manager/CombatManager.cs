using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gdev
{
    public abstract class CombatManager : PlayerManager
    {
        protected abstract void HandleCombat();
        public abstract void HandleAiming(State currentStatestate);
        protected abstract void StateChange(State state);
    }
}
