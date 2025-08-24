using System.Linq;
using UnityEngine;

namespace MacadamiaNuts.Valuables
{
    public class BigNutValuable : MonoBehaviour
    {
        [SerializeField] private float _maxDistance = 2f;
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        [SerializeField] private Sound _meAndTheBirds;

        private PhysGrabObject _physGrabObject;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

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
