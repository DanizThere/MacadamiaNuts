using MacadamiaNuts.Golden;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MacadamiaNuts.Valuables
{
    public class GoldenNutValuable : MonoBehaviour
    {
        [SerializeField] private GameObject _goldenPrefab;
        [SerializeField] private ParticleSystem _shiningParticles;

        private HashSet<PlayerAvatar> _corryptedPlayers = new();

        private List<PlayerAvatar> _players = new();

        private PhysGrabObject _physGrabObject;

        private bool _isGrabbed => _physGrabObject.playerGrabbing.Any();
        private bool _isShining => _shiningParticles.isPlaying;

        private void Awake()
        {
            _physGrabObject = GetComponent<PhysGrabObject>();
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

            _players = _physGrabObject.playerGrabbing.Select(x => x.playerAvatar).ToList();
        }

        private void Corrypt(List<PhysGrabber> players)
        {
            Color possessColor = Color.yellow;

            uint newPlayers = System.Convert.ToUInt32(players.Count - 1 - _players.Count);
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
                        GO.GetComponent<GoldenHead>().Initialize();
                        GO.GetComponent<GoldenHead>().StartCorrypt();
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
    }
}
