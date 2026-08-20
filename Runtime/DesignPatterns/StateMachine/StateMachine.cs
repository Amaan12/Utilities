using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.SimpleStateMachine
{
    /// <summary>
    /// IHeartGameDev's State Machine, unlike git-amend's platformer course's state machine, this doesn't support predicates
    /// Key features
    /// 1. References all states
    /// 2. Oversees the active state
    /// 3. Handles transitions between states
    /// 4. Calls methods of states such EnterState, ExitState, UpdateState, etc.
    /// </summary>
    public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum
    {
        #region Fields
        protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
        protected BaseState<EState> CurrentState;

        protected bool IsTransitioningState = false;
        #endregion

        #region Monobehavior Callbacks
        protected virtual void Start()
        {
            CurrentState?.EnterState();
        }

        protected virtual void Update()
        {
            if (IsTransitioningState)
            {
                return;
            }

            if (TryGetGlobalStateTransition(out EState forcedState))
            {
                if (!forcedState.Equals(CurrentState.StateKey))
                    TransitionToState(forcedState);
                return;
            }

            if (NextStateTransition())
            {
                return;
            }

            CurrentState.UpdateState();
        }

        protected virtual bool TryGetGlobalStateTransition(out EState nextState)
        {
            nextState = default;
            return false;
        }

        protected virtual bool NextStateTransition()
        {
            EState next = CurrentState.GetNextState();
            if (!next.Equals(CurrentState.StateKey))
            {
                TransitionToState(next);
                return true;
            }
            return false;
        }

        protected virtual void TransitionToState(EState nextStateKey)
        {
            IsTransitioningState = true;
            CurrentState.ExitState();
            CurrentState = States[nextStateKey];
            CurrentState.EnterState();
            IsTransitioningState = false;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            CurrentState.OnTriggerEnter(other);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            CurrentState.OnTriggerStay(other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            CurrentState.OnTriggerExit(other);
        }
        #endregion

        #region Custom Functions
        #endregion
    }
}

