using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.ReorderableIfChains
{
    // Delegate for a "Try → Execute" action
    public delegate bool TryAction();

    public class ActionRunner : MonoBehaviour
    {
        // List of actions you can reorder
        private List<TryAction> actions;

        void Start()
        {
            // Initialize actions in default order
            actions = new List<TryAction>
            {
                TrySayHello,
                TrySayGoodbye,
                TrySayRandom,
                () => CondAction(() => UnityEngine.Random.value > 0.5f, () => Debug.Log("Hi"))
            };

            // Run all actions (first valid executes like else-if)
            RunActions();
        }

        /// <summary>
        /// Executes the actions in order like an if/else-if chain.
        /// </summary>
        public void RunActions()
        {
            foreach (var action in actions)
            {
                if (action()) // if action executed, stop
                    break;
            }
        }

        /// <summary>
        /// Example method to reorder actions at runtime.
        /// </summary>
        public void ReverseActions()
        {
            actions.Reverse();
            Debug.Log("Actions reordered!");
        }

        #region ExampleActions
        bool TrySayHello()
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                Debug.Log("Hello!");
                return true; // executed
            }
            return false; // not executed
        }

        bool TrySayGoodbye()
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                Debug.Log("Goodbye!");
                return true;
            }
            return false;
        }

        bool TrySayRandom()
        {
            Debug.Log("Random message executed as fallback.");
            return true; // always executes
        }

        /// <summary>
        /// Helper to create a TryAction from a condition and an execution.
        /// </summary>
        bool CondAction(Func<bool> condition, Action execute)
        {
            if (condition())
            {
                execute();
                return true;
            }
            return false;
        }
        #endregion
    }
}