using System;

namespace Utilities.BlueprintNodes
{
    public class FlipFlop : IBlueprintNode
    {
        bool toggle;
        Action a;
        Action b;

        public FlipFlop(Action a = null, Action b = null, bool toggle = true)
        {
            this.a = a;
            this.b = b;
            this.toggle = toggle;
        }

        public FlipFlop SetA(Action a)
        {
            this.a = a;
            return this;
        }

        public FlipFlop SetB(Action b)
        {
            this.b = b;
            return this;
        }

        public void Reset()
        {
            toggle = false;
        }

        public void Toggle()
        {
            toggle = !toggle;
        }

        public void Execute()
        {
            if (toggle)
                a?.Invoke();
            else
                b?.Invoke();

            toggle = !toggle;
        }
    }
}