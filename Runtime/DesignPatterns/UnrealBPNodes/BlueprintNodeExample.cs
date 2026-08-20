using UnityEngine;

namespace Utilities.BlueprintNodes
{
    public class BlueprintNodeExample : MonoBehaviour
    {
        private DoOnce _doOnce;
        private DoN _doN;
        private FlipFlop _flipFlop;
        private Gate _gate;
        private MultiGate _multiGate;
        private Sequence _sequence;

        private void Start()
        {
            _doOnce = new DoOnce()
                .SetAction(() => Debug.Log("DoOnce executed"));

            _doN = new DoN(3)
                .SetAction(() => Debug.Log("DoN executed"));

            _flipFlop = new FlipFlop()
                .SetA(() => Debug.Log("FlipFlop A"))
                .SetB(() => Debug.Log("FlipFlop B"));

            _gate = new Gate()
                .SetAction(() => Debug.Log("Gate executed"));

            _gate.Close();

            _multiGate = new MultiGate()
                .AddOutput(() => Debug.Log("MultiGate 1"))
                .AddOutput(() => Debug.Log("MultiGate 2"))
                .AddOutput(() => Debug.Log("MultiGate 3"));

            _sequence = new Sequence()
                .AddStep(() => Debug.Log("Sequence Step 1"))
                .AddStep(() => Debug.Log("Sequence Step 2"))
                .AddStep(() => Debug.Log("Sequence Step 3"));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _doOnce.Execute();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _doN.Execute();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _flipFlop.Execute();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                _gate.Execute();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                _gate.Open();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                _gate.Close();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                _multiGate.Execute();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _sequence.Execute();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _doOnce.Reset();
                _doN.Reset();
                _flipFlop.Reset();
                _gate.Reset();
                _multiGate.Reset();
                _sequence.Reset();
            }
        }
    }
}