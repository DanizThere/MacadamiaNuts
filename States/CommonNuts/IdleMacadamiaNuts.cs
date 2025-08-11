using MacadamiaNuts.Timer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MacadamiaNuts.States
{
    public class IdleMacadamiaNuts : MacadamiaState
    {
        private MacadamiaTimer _timer;

        private float _min = 0f;
        private float _max = 0f;

        private bool _isFilledByOne => _layers.Count == 1;
        private bool _isFilled => _layers.Any();    
        private ParticleSystem _particleSystem;

        private Queue<GameObject> _layers = new();
        private Sound _soundCrack;
        PhysGrabObject _physGrabObject;

        public IdleMacadamiaNuts(MacadamiaTimerData macadamiaTimerData, ParticleSystem particleSystem, Queue<GameObject> layers, Sound soundCrack, PhysGrabObject physGrabObject)
        {
            _min = macadamiaTimerData.Min;
            _max = macadamiaTimerData.Max;

            _timer = new(macadamiaTimerData.StartTime);

            _particleSystem = particleSystem;
            _layers = layers;
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {
            _timer.UpdateCounter(Random.Range(_min, _max));
        }

        public override void Update()
        {
            _timer.Countdown(Time.deltaTime, () =>
            {
                NutsCrackes();

                _timer.UpdateCounter(Random.Range(_min, _max));
            });
        }

        private void NutsCrackes()
        {
            _particleSystem.Play();

            if (_isFilledByOne)
                _physGrabObject.impactDetector.DestroyObject(true);

            if (!_isFilled)
            {
                var go = _layers.Dequeue();
                go.SetActive(false);

                _physGrabObject.impactDetector.BreakMedium(_physGrabObject.centerPoint, true);
                _soundCrack.Play(_physGrabObject.centerPoint);
            }
        }
    }
}
