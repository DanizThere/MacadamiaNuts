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

        [SerializeField] private MacadamiaTimerData _activeTimerData;
        [SerializeField] private MacadamiaTimerData _idleTimerData;

        [SerializeField] private GameObject _firstLayer;
        [SerializeField] private GameObject _secondLayer;
        [SerializeField] private GameObject _thirdLayer;

        [SerializeField] private Sound _soundCrack;
        [SerializeField] private Sound _semiEat;

        private Queue<GameObject> _nutsLayers = new();
        private List<string> _objections = new();

        private ParticleSystem _particle;

        private PhysGrabObject _physGrabObject;
        private PhotonView _photonView;
        private PhysGrabCart _cart;

        private MacadamiaStateMachine _stateMachine;

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
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            var activeState = new ActiveMacadamiaNuts(_activeTimerData, damage: 1, _semiEat, _physGrabObject, _photonView, _nutsLayers, _particle);
            var idleState = new IdleMacadamiaNuts(_idleTimerData, _particle, _nutsLayers, _soundCrack, _physGrabObject, _cart, transform, MAX_ANGLE);
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

        private void InitializeObjections()
        {
            var objections = new string[] { " , fuk, ma macafamia nuff ar clumflinf" };

            _objections.AddRange(objections);
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