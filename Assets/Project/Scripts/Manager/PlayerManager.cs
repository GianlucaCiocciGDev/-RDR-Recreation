using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gdev
{
    public abstract class PlayerManager : MonoBehaviour
    {
        protected InputManager inputManager;
        protected virtual void Awake()
        {
            inputManager = GetComponent<InputManager>();
        }
    }
}
