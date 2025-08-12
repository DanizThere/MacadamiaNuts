using System.Collections.Generic;
using UnityEngine;
using System;

namespace MacadamiaNuts.States
{
    public class MacadamiaStateMachine
    {
        private Dictionary<string, MacadamiaState> _states = new();

        private MacadamiaState _currentState;

        public void AddState(MacadamiaState state)
        {
            var stateName = state.ToString();

            if(!_states.ContainsKey(stateName))
            {
                _states.Add(stateName, state);

                Debug.Log($"New state ({stateName}) was added.");
            }
        }

        public void SetState<T>() where T : MacadamiaState
        {
            var stateName = typeof(T).Name;

            if(_states.TryGetValue(stateName, out var state))
            {
                if (MacadamiaState.Equals(state, _currentState)) return;

                _currentState?.Exit();
                _currentState = state;
                _currentState.Enter();

                Debug.Log($"This state ({stateName}) is current");

                return;
            }

            Debug.LogWarning($"This state ({stateName}) doesnt exist");
        }

        public void ExecuteState()
        {
            _currentState?.Update();
        }
    }
}
