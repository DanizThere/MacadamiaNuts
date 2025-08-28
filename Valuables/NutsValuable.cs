using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.Collections;
using MacadamiaNuts.Timer;
using MacadamiaNuts.States;
using System.Linq;
using MacadamiaNuts.States.CommonNuts;

namespace MacadamiaNuts.Valuables
{
    public class NutsValuable : MonoBehaviour
    {
        private const float MAX_ANGLE = 80f;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        [SerializeField] private MacadamiaTimerData _activeTimerData;
        [SerializeField] private MacadamiaTimerData _idleTimerData;

        [SerializeField] private GameObject _firstLayer;
        [SerializeField] private GameObject _secondLayer;
        [SerializeField] private GameObject _thirdLayer;

        [SerializeField] private Sound _soundCrack;
        [SerializeField] private Sound _semiEat;

        private ParticleSystem _particle;

        private PhysGrabObject _physGrabObject;
        private PhotonView _photonView;
        private PhysGrabCart _cart;

        private MacadamiaStateMachine _stateMachine;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        private Queue<GameObject> _nutsLayers = new();
        private List<string> _objections = new();

        private bool _isGrabbed => _physGrabObject.playerGrabbing.Any();

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _particle = GetComponentInChildren<ParticleSystem>();
            _physGrabObject = GetComponent<PhysGrabObject>();

            _nutsLayers.Enqueue(_firstLayer);
            _nutsLayers.Enqueue(_secondLayer);
            _nutsLayers.Enqueue(_thirdLayer);

            StartCoroutine(TryGetCart());

            _stateMachine = new();

            var activeState = GetComponent<ActiveMacadamiaNuts>();
            activeState.Initialize(_activeTimerData, damage: 1, _semiEat, _physGrabObject, _photonView, _nutsLayers, _particle);

            var idleState = GetComponent<IdleMacadamiaNuts>();
            idleState.Initialize(_idleTimerData, _particle, _nutsLayers, _soundCrack, _physGrabObject, _cart, transform, MAX_ANGLE);

            _stateMachine.AddState(activeState);
            _stateMachine.AddState(idleState);
        }

        private void Update()
        {
            if (_isGrabbed)
            {
                _stateMachine.SetState<ActiveMacadamiaNuts>();
            }
            else if (!_isGrabbed) _stateMachine.SetState<IdleMacadamiaNuts>();

            _stateMachine.ExecuteState();
        }

        public void NutsCracked()
        {
            _particle.Play();
        }

        private IEnumerator TryGetCart()
        {
            int attempts = 5;

            for(int i = 0; i < attempts; i++)
            {
                _cart = FindAnyObjectByType<PhysGrabCart>();

                if(_cart != null)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}