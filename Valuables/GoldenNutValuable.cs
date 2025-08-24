using MacadamiaNuts.Golden;
using System.Collections.Generic;
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

        private HashSet<PlayerAvatar> _corryptedPlayers = new();

        private HashSet<PlayerAvatar> _players = new();

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

                UpdatePlayerList();

                return;
            }

            TryCorrypt();
        }

        public void RemoveCorryptedPlayer(PlayerAvatar avatar)
        {
            if(_corryptedPlayers.Contains(avatar)) _corryptedPlayers.Remove(avatar);
        }

        private void UpdatePlayerList()
        {
            _players.Clear();

            _players = _physGrabObject.playerGrabbing.Select(x => x.playerAvatar).ToHashSet();
        }

        private void Corrypt(List<PhysGrabber> players)
        {
            var newPlayers = System.Convert.ToUInt32(players.Count - 1 - _players.Count);
            for (uint i = newPlayers; i < players.Count; i++)
            {
                var avatar = players[(int)i].playerAvatar;
                var avatarVisual = players[(int)i].transform.parent.GetComponentInChildren<PlayerAvatarVisuals>();

                var existGold = avatar.GetComponentInChildren<GoldenHead>();
                if(existGold != null)
                {
                    existGold.IncreaseCorryption();
                }
                else
                {
                    if (_corryptedPlayers.Add(avatar))
                    {
                        var GO = Instantiate(_goldenPrefab, avatar.transform);

                        GO.GetComponent<GoldenPlayerAvatar>().Initialize(avatar, avatarVisual, this);
                        GO.GetComponent<GoldenHead>().Initialize(avatar, _corryptSound, _startCorryptSound);
                    }
                }
            }

            UpdatePlayerList();
        }

        private void TryCorrypt()
        {
            var players = _physGrabObject.playerGrabbing;

            if (players.Count <= _players.Count)
            {
                UpdatePlayerList();

                return;
            }

            Corrypt(players);
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
