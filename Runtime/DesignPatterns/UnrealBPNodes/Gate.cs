using System;
using System.Collections.Generic;

namespace Utilities.BlueprintNodes
{
    public class Gate : IBlueprintNode
    {
        bool _isOpen;
        readonly Queue<Action> _buffer = new();

        Action _onExecute;

        public Gate(Action onExecute = null)
        {
            _onExecute = onExecute;
        }

        public Gate SetAction(Action onExecute)
        {
            _onExecute = onExecute;
            return this;
        }

        public void Open()
        {
            _isOpen = true;

            // flush buffered executions
            while (_buffer.Count > 0)
            {
                _buffer.Dequeue()?.Invoke();
            }
        }

        public void Close()
        {
            _isOpen = false;
        }

        public void Toggle()
        {
            _isOpen = !_isOpen;
        }

        public void Reset()
        {
            _isOpen = false;
            _buffer.Clear();
        }

        public void Execute()
        {
            if (_isOpen)
            {
                _onExecute?.Invoke();
            }
            else
            {
                _buffer.Enqueue(_onExecute);
            }
        }
    }
}