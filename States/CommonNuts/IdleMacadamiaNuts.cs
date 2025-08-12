using MacadamiaNuts.Timer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MacadamiaNuts.States.CommonNuts
{
    public class IdleMacadamiaNuts : MacadamiaState
    {
        private readonly float MAX_ANGLE;
        private Transform _objectTransform;

        private Queue<GameObject> _layers = new();
        private PhysGrabCart _cart;

        private MacadamiaTimer _timer;

        private float _min = 0f;
        private float _max = 0f;

        private bool _isFilledByOne => _layers.Count == 1;
        
        private ParticleSystem _particleSystem;

        private Sound _soundCrack;
        PhysGrabObject _physGrabObject;

        private bool _isObjectInCart => _cart.itemsInCart.Contains(_physGrabObject);
        private bool _isRotatedDown => _objectTransform.transform.rotation.eulerAngles.x > MAX_ANGLE || _objectTransform.transform.rotation.eulerAngles.z > MAX_ANGLE;

        public IdleMacadamiaNuts(MacadamiaTimerData macadamiaTimerData, ParticleSystem particleSystem, Queue<GameObject> layers, Sound soundCrack, PhysGrabObject physGrabObject, PhysGrabCart physGrabCart, Transform transform, float angle)
        {
            _min = macadamiaTimerData.Min;
            _max = macadamiaTimerData.Max;

            _timer = new(macadamiaTimerData.StartTime);

            _particleSystem = particleSystem;
            _layers = layers;
            MAX_ANGLE = angle;
            _objectTransform = transform;
            _cart = physGrabCart;
            _physGrabObject = physGrabObject;
            _soundCrack = soundCrack;
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
            if (!_isObjectInCart && _isRotatedDown)
            {
                _timer.Countdown(Time.deltaTime, () =>
                {
                    NutsCrackes();

                    _timer.UpdateCounter(Random.Range(_min, _max));
                });
            }
        }

        private void NutsCrackes()
        {
            _particleSystem.Play();

            if (_isFilledByOne)
                _physGrabObject.impactDetector.DestroyObject(true);

            if (_layers.Any())
            {
                var go = _layers.Dequeue();
                go.SetActive(false);

                _physGrabObject.impactDetector.BreakMedium(_physGrabObject.centerPoint, true);
                _soundCrack.Play(_physGrabObject.centerPoint);
            }
        }
    }
}
