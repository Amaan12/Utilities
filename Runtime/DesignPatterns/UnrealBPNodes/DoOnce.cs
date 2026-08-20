using System;

namespace Utilities.BlueprintNodes
{
    public class DoOnce : IBlueprintNode
    {
        bool _hasExecuted;
        Action _onExecute;

        public DoOnce(Action onExecute = null)
        {
            _onExecute = onExecute;
        }

        public DoOnce SetAction(Action onExecute)
        {
            _onExecute = onExecute;
            return this;
        }

        public void Reset()
        {
            _hasExecuted = false;
        }

        public void Execute()
        {
            if (_hasExecuted)
                return;

            _hasExecuted = true;
            _onExecute?.Invoke();
        }
    }
}