using System;
using System.Collections.Generic;

namespace Utilities.BlueprintNodes
{
    public class Sequence : IBlueprintNode
    {
        private readonly List<Action> _steps = new();

        public Sequence(params Action[] steps)
        {
            _steps?.AddRange(steps);
        }

        public Sequence SetSteps(params Action[] steps)
        {
            _steps.Clear();
            _steps?.AddRange(steps);
            return this;
        }

        public Sequence AddStep(Action step)
        {
            _steps.Add(step);
            return this;
        }

        public void Reset()
        {
            // Sequence has no internal state in Blueprint semantics
            // Included for interface consistency
        }

        public void Execute()
        {
            for (int i = 0; i < _steps.Count; i++)
            {
                _steps[i]?.Invoke();
            }
        }
    }
}