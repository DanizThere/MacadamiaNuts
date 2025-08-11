using System;

namespace MacadamiaNuts.Timer
{
    public class MacadamiaTimer(float baseCounter)
    {
        private float _timer = 0f;
        private float _counter = baseCounter;

        public void UpdateCounter(float newValue)
        {
            _counter = newValue;
            _timer = 0;
        }

        public void Countdown(float deltaTime, Action onEnd = null)
        {
            if(_timer < _counter)
            {
                _timer += deltaTime;
            }

            onEnd?.Invoke();
        }
    }
}
