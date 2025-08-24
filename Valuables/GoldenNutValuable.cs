using MacadamiaNuts.Golden;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private NotifiableList<PlayerAvatar> _players = new();

        private bool _isGrabbed => _physGrabObject.playerGrabbing.Any();
        private bool _isShining => _shiningParticles.isPlaying;

        

        private void Awake()
        {
            _physGrabObject = GetComponent<PhysGrabObject>();
            OnPlayerDeath.AddListener(SummonMacadamiaNuts);

            _players.OnItemAdded += StartCorrypt;
            _players.OnItemAddedAgain += Corrypt;
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

                return;
            }

            TryCorrypt();
        }

        public void RemoveCorryptedPlayer(PlayerAvatar avatar)
        {
            if(_players.Contains(avatar)) _players.Remove(avatar);
        }

        private void TryCorrypt()
        {
            var players = _physGrabObject.playerGrabbing;

            if (players.Count <= _players.Count)
            {
                return;
            }

            foreach (var player in players)
            {
                _players.Add(player.playerAvatar);
            }
        }

        private void Corrypt(PlayerAvatar avatar)
        {
            avatar.transform.parent.GetComponentInChildren<GoldenHead>().IncreaseCorryption();
            avatar.GetComponent<GoldenPlayerAvatar>().UpdateCorryptioner(this);
        }

        private void StartCorrypt(PlayerAvatar avatar)
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
            var newNuts = Instantiate(_macadamiaNuts);
            var physObject = newNuts.GetComponent<PhysGrabObject>();
            newNuts.transform.position = transform.position;
            newNuts.GetComponent<ValuableObject>().DollarValueSetLogic();

            if (_physGrabObject.playerGrabbing.Any())
            {
                foreach(var players in _physGrabObject.playerGrabbing)
                {
                    players.OverrideGrab(physObject);
                }
            }

            Destroy(gameObject);
        }
    }
}


public class NotifiableList<T> : List<T>
{
    public event System.Action<T> OnItemAdded;
    public event System.Action<T> OnItemAddedAgain;

    public new void Add(T item)
    {
        if(Contains(item))
        {
            OnItemAddedAgain?.Invoke(item);
            return;
        }

        base.Add(item);
        OnItemAdded?.Invoke(item);
    }
}
