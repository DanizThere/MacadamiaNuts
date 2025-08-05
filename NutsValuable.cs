using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.Collections;

namespace MacadamiaNuts
{
    public class NutsValuable : MonoBehaviour
    {
        private const float MAX_ANGLE = 80f;

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
        private PhysGrabCart _cart;

        private bool _isGrabbed;
        private bool IsEmpty => _nutsLayers.Count == 0;

        private bool _isHurting;
        private bool _isObjectInCart;


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

            StartCoroutine(TryGetCart());
        }

        private void Update()
        {
            if (!_cart) return;

            _isObjectInCart = _cart.itemsInCart.Contains(_physGrabObject);

            _isGrabbed = _physGrabObject.playerGrabbing.Count > 0;

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

            if(_nutsLayers.Count == 1)
                _physGrabObject.impactDetector.DestroyObject(true);

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
            if(_isGrabbed)
            {
                _currentState = State.Active;
                return;
            }

            if (_isObjectInCart) return;

            var isLookDown = transform.rotation.eulerAngles.x > MAX_ANGLE || transform.rotation.eulerAngles.z > MAX_ANGLE;

            if (isLookDown && _nutsLayers.Count != 0)
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
            if (!_isGrabbed)
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

                Emit(15, player.playerAvatar.spectatePoint);
            }

            _isHurting = false;
        }

        private void InitializeObjections()
        {
            var objections = new string[] { " , fuk, ma macafamia nuff ar clumflinf" };

            _objections.AddRange(objections);
        }

        private void Emit(int count, Transform transform)
        {
            for(int i = 0; i < count; i++)
            {
                Emit(transform);
            }
        }

        private void Emit(Transform basePoint)
        {
            var newPosition = basePoint.position + (basePoint.forward * .3f); 
            newPosition += new Vector3(Random.Range(-.1f, .1f), Random.Range(-.05f, .1f), 0);

            var emitParams = new ParticleSystem.EmitParams
            {
                position = newPosition,
                velocity = Vector3.up * .2f
            };

            _particle.Emit(emitParams, 1);
        }

        private void HurtPlayer()
        {
            if (SemiFunc.IsMasterClientOrSingleplayer())
            {
                if (SemiFunc.IsMultiplayer())
                {
                    _photonView.RPC(nameof(HurtPlayerRPC), RpcTarget.All);
                }
                else
                {
                    HurtPlayerRPC();
                }
            }
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
                yield return new WaitForSeconds(1);
            }
        }

        [PunRPC]
        private void HurtPlayerRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
        {
            if (SemiFunc.MasterOnlyRPC(_info))
            {
                _isHurting = true;
            }
        }
    }
}