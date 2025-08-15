using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MacadamiaNuts.Valuables
{
    public class GoldenNutValuable : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _baseParticleSystem;
        [SerializeField] private ParticleSystem _shiningParticles;

        private HashSet<PlayerAvatar> _corryptedPlayers = new();

        private PhysGrabObject _physGrabObject;
        private PhotonView _photonView;

        private bool _isGrabbed => _physGrabObject.playerGrabbing.Any();
        private bool _isShining => _shiningParticles.isPlaying;

        private void Awake()
        {
            _physGrabObject = GetComponent<PhysGrabObject>();
            _photonView = GetComponent<PhotonView>();
        }

        private void Update()
        {
            if(!_isGrabbed)
            {
                if (!_isShining)
                {
                    _shiningParticles.Play();
                }
            }
            else
            {
                var players = _physGrabObject.playerGrabbing;

                foreach(var player in players)
                {
                    var avatar = player.playerAvatar;
                    if (_corryptedPlayers.Add(avatar))
                    {
                        if(!avatar.TryGetComponent<GoldenPlayerAvatar>(out var _))
                        {
                            var gold = avatar.gameObject.AddComponent<GoldenPlayerAvatar>();

                            gold.Initialize();
                        }
                        else
                        {

                        }
                    }
                }
            }
        }
    }
}
