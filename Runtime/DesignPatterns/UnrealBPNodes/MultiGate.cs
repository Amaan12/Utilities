using System;
using System.Collections.Generic;

namespace Utilities.BlueprintNodes
{
    public class MultiGate : IBlueprintNode
    {
        private int _index;
        private readonly List<Action> _outputs = new();

        public bool Loop { get; set; } = true;
        public bool Randomize { get; set; } = false;

        private readonly Random _rng = new();

        public MultiGate(params Action[] outputs)
        {
            _outputs?.AddRange(outputs);
        }

        public MultiGate SetOutputs(params Action[] outputs)
        {
            _outputs.Clear();
            _outputs?.AddRange(outputs);

            _index = 0;
            return this;
        }

        public MultiGate AddOutput(Action output)
        {
            _outputs.Add(output);
            return this;
        }

        public void Reset()
        {
            _index = 0;
        }

        public void Execute()
        {
            if (_outputs.Count == 0)
                return;

            int chosenIndex;

            if (Randomize)
            {
                chosenIndex = _rng.Next(_outputs.Count);
            }
            else
            {
                chosenIndex = _index;
            }

            _outputs[chosenIndex]?.Invoke();

            if (!Randomize)
            {
                _index++;

                if (_index >= _outputs.Count)
                {
                    _index = Loop ? 0 : _outputs.Count - 1;
                }
            }
        }
    }
}