using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.SimpleStateMachine
{
    public abstract class BaseState<EState> where EState : Enum
    {
        #region Fields
        public EState StateKey { get; private set; }
        #endregion

        #region Constructor
        public BaseState(EState key)
        {
            StateKey = key;
        }
        #endregion

        #region Custom Functions
        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void UpdateState();

        public abstract EState GetNextState();

        public abstract void OnTriggerEnter(Collider other);
        public abstract void OnTriggerStay(Collider other);
        public abstract void OnTriggerExit(Collider other);
        #endregion
    }
}
