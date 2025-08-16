using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MacadamiaNuts.Valuables
{
    public class GoldenNutValuable : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _shiningParticles;
        [SerializeField] private Sound _corryptSound;

        private HashSet<PlayerAvatar> _corryptedPlayers = new();

        private List<PlayerAvatar> _players = new();

        private PhysGrabObject _physGrabObject;

        private bool _isGrabbed => _physGrabObject.playerGrabbing.Any();
        private bool _isShining => _shiningParticles.isPlaying;

        private void Awake()
        {
            _physGrabObject = GetComponent<PhysGrabObject>();

            print(GetComponentInChildren<Renderer>().material.shader.name);
            print(GetComponentInChildren<Renderer>().material.shader);
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
            uint newPlayers = System.Convert.ToUInt32(players.Count - 1 - _players.Count);
            for (uint i = newPlayers; i < players.Count; i++)
            {
                var avatar = players[(int)i].playerAvatar;

                var existGold = avatar.GetComponentInChildren<GoldenPlayerAvatar>();
                if(existGold != null)
                {
                    existGold.IncreaseCorryption();
                    _corryptSound.Play(avatar.transform.position);
                }
                else
                {
                    if (_corryptedPlayers.Add(avatar))
                    {
                        var GO = new GameObject("GoldenCorryption");
                        GO.transform.SetParent(avatar.transform);

                        var gold = GO.AddComponent<GoldenPlayerAvatar>();
                        gold.Initialize(avatar, this);

                        //ChatManager.instance.PossessChatScheduleStart(15);
                        //ChatManager.instance.PossessChat(ChatManager.PossessChatID.None, "Oh, this is a very creepy feeling >//<", typingSpeed: 1f, possessColor);
                        //ChatManager.instance.PossessChatScheduleEnd();
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
