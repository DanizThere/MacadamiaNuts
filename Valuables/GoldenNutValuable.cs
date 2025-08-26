using MacadamiaNuts.Golden;
using REPOLib.Modules;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MacadamiaNuts.Valuables
{
    public class GoldenNutValuable : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent OnPlayerDeath = new();

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        [SerializeField] private GameObject _goldenPrefab;
        [SerializeField] private ParticleSystem _shiningParticles;
        [SerializeField] private GameObject _macadamiaNuts;

        [SerializeField] private Sound _corryptSound;
        [SerializeField] private Sound _startCorryptSound;

        private PhysGrabObject _physGrabObject;

#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        private bool _isGrabbed => _physGrabObject.playerGrabbing.Any();
        private bool _isShining => _shiningParticles.isPlaying;

        
        private void Awake()
        {
            _physGrabObject = GetComponent<PhysGrabObject>();
            OnPlayerDeath.AddListener(SummonMacadamiaNuts);
        }

        private void OnDestroy()
        {
            OnPlayerDeath.RemoveAllListeners();
        }

        private void Update()
        {
            if(!_isGrabbed)
            {
                if (!_isShining)
                {
                    _shiningParticles.Play();
                }
                else _shiningParticles.Stop();
            }
        }

        private void Corrypt(PlayerAvatar avatar)
        {
            var goldenHead = avatar.transform.parent.GetComponentInChildren<GoldenHead>();
            goldenHead.IncreaseCorryption();
            goldenHead.GetComponent<GoldenPlayerAvatar>().UpdateCorryptioner(this);
        }

        public void StartCorrypt(PlayerAvatar avatar)
        {
            var goldenHead = avatar.transform.parent.GetComponentInChildren<GoldenHead>();

            if(goldenHead != null)
            {
                Corrypt(avatar);
                return;
            }

            var avatarVisual = avatar.transform.parent.GetComponentInChildren<PlayerAvatarVisuals>();

            var GO = Instantiate(_goldenPrefab, avatar.transform);
            GO.GetComponent<GoldenPlayerAvatar>().Initialize(avatar, avatarVisual, this);
            GO.GetComponent<GoldenHead>().Initialize(avatar, _corryptSound, _startCorryptSound);
        }

        private void SummonMacadamiaNuts()
        {
            var position = transform.position;
            var rotation = transform.rotation;

            GameObject GO = null;

            if (SemiFunc.IsMultiplayer())
            {
                if (_macadamiaNuts.TryGetComponent<ValuableObject>(out var valuable))
                    GO = REPOLib.Modules.Valuables.SpawnValuable(valuable, position, rotation);
            }
            else
            {
                GO = UnityEngine.Object.Instantiate(_macadamiaNuts, position, rotation);
            }

            var physGrabObject = GO.GetComponent<PhysGrabObject>();

            if (_physGrabObject.playerGrabbing.Any())
            {
                foreach(var players in _physGrabObject.playerGrabbing)
                {
                    players.OverrideGrab(physGrabObject);
                }
            }

            Destroy(gameObject);
        }
    }
}
