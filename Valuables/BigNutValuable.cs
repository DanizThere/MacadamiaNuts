using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MacadamiaNuts.Valuables
{
    public class BigNutValuable : MonoBehaviour
    {
        [SerializeField] private float _maxDistance = 2f;
        [SerializeField] private Sound _meAndTheBirds;

        private PhysGrabObject _physGrabObject;

        private bool _isPushing;

        private void Awake()
        {
            _physGrabObject = GetComponent<PhysGrabObject>();
        }


        private void Update()
        {
            if (_physGrabObject.playerGrabbing.Any())
            {
                foreach(var players in _physGrabObject.playerGrabbing.ToList())
                {
                    players.ReleaseObject();
                }
            }

            _meAndTheBirds.PlayLoop(_isPushing, fadeInSpeed: .5f, fadeOutSpeed: 1);

            var cols = Physics.OverlapSphere(transform.position, _maxDistance);
            foreach (var col in cols)
            {
                if (col.gameObject.CompareTag("Player"))
                {
                    _isPushing = true;

                    break;
                }
                else _isPushing = false;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _maxDistance);
        }

    }
}
