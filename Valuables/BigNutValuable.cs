using Photon.Pun;
using UnityEngine;

namespace MacadamiaNuts.Valuables
{
    public class BigNutValuable : MonoBehaviour
    {
        [SerializeField] private Sound _meAndTheBirds;
        [SerializeField] private float _throwForce = 10f;

        private PhysGrabObject _physGrabObject;
        private PhotonView _photonView;

        private bool _isPushing;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _physGrabObject = GetComponent<PhysGrabObject>();
        }

        private void Update()
        {
            if (_physGrabObject.grabbed) _physGrabObject.playerGrabbing.Clear();

            _meAndTheBirds.PlayLoop(_isPushing, fadeInSpeed: 1, fadeOutSpeed: 1);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.TryGetComponent<PlayerAvatar>(out var _))
            {
                _isPushing = true;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if(collision.transform.TryGetComponent<PlayerAvatar>(out var player))
            {
                _physGrabObject.rb.AddForce(player.transform.forward * _throwForce, ForceMode.Impulse);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.transform.TryGetComponent<PlayerAvatar>(out var _))
            {
                _isPushing = false;
            }
        }
    }
}
