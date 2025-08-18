using MacadamiaNuts.Timer;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MacadamiaNuts.States.CommonNuts
{
    public class ActiveMacadamiaNuts : MacadamiaState
    {
        private Queue<GameObject> _layers = new();

        private MacadamiaTimer _timer;
        private float _min = 0f;
        private float _max = 0f;

        private int _damage = 1;

        private PhysGrabObject _grabObject;
        private PhotonView _photonView;
        private bool _isHurting;

        private Sound _semiEat;
        private ParticleSystem _particleSystem;

        private bool _isFilled => _layers.Any();

        public void Initialize(MacadamiaTimerData macadamiaTimerData, int damage, Sound semiEat, PhysGrabObject physGrabObject, PhotonView photonView, Queue<GameObject> layers, ParticleSystem particleSystem)
        {
            _min = macadamiaTimerData.Min;
            _max = macadamiaTimerData.Max;

            _grabObject = physGrabObject;

            _timer = new(macadamiaTimerData.StartTime);

            _damage = damage;
            _photonView = photonView;
            _semiEat = semiEat;

            _particleSystem = particleSystem;
            _layers = layers;
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {
            _timer.UpdateCounter(Random.Range(_min, _max));
        }

        public override void Execute()
        {
            if (_isFilled)
            {
                if (_isHurting)
                {
                    HurtPlayerEffect();
                }
            }

            _timer.Countdown(Time.deltaTime, () =>
            {
                var players = _grabObject.playerGrabbing;

                if (players.Any())
                {
                    HurtPlayer();
                }

                _timer.UpdateCounter(Random.Range(_min, _max));
            });
        }

        private void HurtPlayerEffect()
        {
            foreach (var player in _grabObject.playerGrabbing)
            {
                _semiEat.Stop();

                player.playerAvatar.playerHealth.Hurt(_damage, false);
                _semiEat.Play(player.transform.position);

                if (_isFilled)
                    _damage = Random.Range(1, 3);

                Emit(15, player.playerAvatar.spectatePoint);
            }

            _isHurting = false;
        }

        private void Emit(int count, Transform transform)
        {
            for (int i = 0; i < count; i++)
            {
                Emit(transform);
            }
        }

        private void Emit(Transform basePoint)
        {
            var newPosition = basePoint.position + basePoint.forward * .3f;
            newPosition += new Vector3(Random.Range(-.1f, .1f), Random.Range(-.05f, .1f), 0);

            var emitParams = new ParticleSystem.EmitParams
            {
                position = newPosition,
                velocity = Vector3.up * .2f
            };

            _particleSystem.Emit(emitParams, 1);
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

        [PunRPC]
        private void HurtPlayerRPC(PhotonMessageInfo _info = default)
        {
            if (SemiFunc.MasterOnlyRPC(_info))
            {
                _isHurting = true;
            }
        }
    }
}
