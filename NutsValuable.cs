using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

namespace MacadamiaNuts
{
    public class NutsValuable : MonoBehaviour
    {
        private const float MAX_ANGLE = 80f;

        [SerializeField] private AudioSource _metalPipe;
        [SerializeField] private GameObject _firstLayer;
        [SerializeField] private GameObject _secondLayer;
        [SerializeField] private GameObject _thirdLayer;

        [SerializeField] private Sound _soundCrack;
        [SerializeField] private Sound _semiEat;

        private Queue<GameObject> _nutsLayers = new();
        private List<string> _objections = new();

        private ParticleSystem _particle;

        private State _currentState;

        private float _cooldownUntilNextLayer = 3f;
        private float _cooldownUntilNextHit = 3f;

        private int _damage = 1;

        private PhysGrabObject _physGrabObject;
        private PhotonView _photonView;

        private bool IsGrabbed => PhysGrabber.instance.grabbedPhysGrabObject == _physGrabObject && PhysGrabber.instance.grabbed && _physGrabObject.grabbed;
        private bool IsEmpty => _nutsLayers.Count == 0;

        private bool _isHurting;

        private enum State
        {
            Idle,
            Active
        }

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _particle = GetComponentInChildren<ParticleSystem>();
            _physGrabObject = GetComponent<PhysGrabObject>();

            _nutsLayers.Enqueue(_firstLayer);
            _nutsLayers.Enqueue(_secondLayer);
            _nutsLayers.Enqueue(_thirdLayer);

            _currentState = State.Idle;
        }

        private void Update()
        {
            if (!IsEmpty)
            {
                if (_isHurting)
                {
                    HurtPlayerEffect();
                }
            }

            switch (_currentState)
            {
                case State.Idle:
                    StateIdle();
                    break;
                case State.Active:
                    StateActive();
                    break;
            }
        }

        public void NutsCracked()
        {
            _particle.Play();
        }

        private void NutsCrackes()
        {
            _particle.Play();
            if (_nutsLayers.Count != 0)
            {
                var go = _nutsLayers.Dequeue();
                go.SetActive(false);

                _physGrabObject.impactDetector.BreakMedium(_physGrabObject.centerPoint, true);
                _soundCrack.Play(_physGrabObject.centerPoint);
            }
        }

        private void StateIdle()
        {
            if(IsGrabbed)
            {
                _currentState = State.Active;
                return;
            }

            if (transform.rotation.eulerAngles.x > MAX_ANGLE && _nutsLayers.Count != 0)
            {
                if (_cooldownUntilNextLayer > 0)
                {
                    _cooldownUntilNextLayer -= Time.deltaTime;
                }
                else
                {
                    NutsCrackes();
                    _cooldownUntilNextLayer = Random.Range(3f, 5f);
                }
            }
        }

        private void StateActive()
        {
            if (!IsGrabbed)
            {
                _currentState = State.Idle;
                return;
            }

            if (_cooldownUntilNextHit > 0)
            {
                _cooldownUntilNextHit -= Time.deltaTime;
            }
            else
            {
                HurtPlayer();
            }
        }

        private void HurtPlayerEffect()
        {
            foreach (var player in _physGrabObject.playerGrabbing)
            {
                _semiEat.Stop();

                player.playerAvatar.playerHealth.Hurt(_damage, false);
                _semiEat.Play(player.transform.position);

                _cooldownUntilNextHit = Random.Range(3f, 5f);

                if (_nutsLayers.Count > 0)
                    _damage = Random.Range(1, 3);
                else _damage = Random.Range(5, 10);

                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = player.playerAvatar.spectatePoint.position + (player.playerAvatar.spectatePoint.forward * .3f);
                emitParams.angularVelocity3D = Vector3.down;

                _particle.Emit(emitParams, 15);
            }

            _isHurting = false;
        }

        private void InitializeObjections()
        {
            var objections = new string[] { " , fuk, ma macafamia nuff ar clumflinf" };

            _objections.AddRange(objections);
        }

        private void HurtPlayer()
        {
            if (SemiFunc.IsMasterOrSingleplayer())
            {
                if (SemiFunc.IsMultiplayer())
                {
                    _photonView.RPC(nameof(), RpcTarget.All);
                }
                else
                {
                    HurtPlayerRPC();
                }
            }
        }

        [RunRPC]
        private void HurtPlayerRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
        {
            if (SemiFunc.MasterOnlyRPC(_info))
            {
                _isHurting = true;
            }
        }
    }
}