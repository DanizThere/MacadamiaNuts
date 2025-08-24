using System.Collections.Generic;
using UnityEngine;
using System;

namespace MacadamiaNuts.States
{
    public class MacadamiaStateMachine
    {
        private Dictionary<string, MacadamiaState> _states = new();

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private MacadamiaState _currentState;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        public void AddState(MacadamiaState state)
        {
            var stateName = state.ToString();

            if(!_states.ContainsKey(stateName))
            {
                _states.Add(stateName, state);
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

                return;
            }

            Debug.LogWarning($"This state ({stateName}) doesnt exist");
        }

        public void ExecuteState()
        {
            _currentState?.Execute();
        }
    }
}
