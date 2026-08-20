using System;

namespace Utilities.BlueprintNodes
{
    public class DoN : IBlueprintNode
    {
        int _count;
        Action _onExecute;

        public int MaxCount { get; private set; }

        public DoN(int maxCount = 1, Action onExecute = null)
        {
            MaxCount = maxCount;
            _onExecute = onExecute;
        }

        public DoN SetAction(Action onExecute)
        {
            _onExecute = onExecute;
            return this;
        }

        public void SetMax(int maxCount)
        {
            MaxCount = Math.Max(0, maxCount);
        }

        public void Reset()
        {
            _count = 0;
        }

        public void Execute()
        {
            if (_count >= MaxCount)
                return;

            _count++;
            _onExecute?.Invoke();
        }
    }
}