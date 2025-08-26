using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MacadamiaNuts.Golden
{
    public class GoldenVingette : MonoBehaviour
    {
        private const string VINGETTE_KEY = "_MinValue";
        private const float MIN_VALUE = .4f;
        private const float MAX_VALUE = .8f;

        [SerializeField] private Color _red;
        [SerializeField] private float _vingetteMove = 10f;
        private Color _baseColor;

        private float _counter;
        private float _max;

        private bool _isUpdateVingette;
        private float _currentVignette;
        private float _sinVignette = MAX_VALUE;

        private IEnumerator _coroutine;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Image _vingette;

        private Material _materialPrefab;
        private Material _material;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        private void Awake()
        {
            _vingette = GetComponentInChildren<Image>();
            _materialPrefab = _vingette.material;

            _material = new(_materialPrefab)
            {
                name = "personal vignette"
            };
            _vingette.material = _material;
            _baseColor = _material.GetColor("_Color");

            if (!_material.HasProperty(VINGETTE_KEY))
            {
                Debug.Log($"This material doesnt has property: {VINGETTE_KEY}");

                return;
            }

            _material.SetFloat(VINGETTE_KEY, MAX_VALUE);

            Hide();
        }

        private void Update()
        {
            if (_isUpdateVingette)
            {
                return;
            }

            ShakeVingetteUpdate();
        }

        public void Show()
        {
            _vingette.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _vingette.gameObject.SetActive(false);
        }

        public void ResetVingette()
        {
            ShowCurrentVignette(MAX_VALUE, MAX_VALUE);
        }

        public void ShowCurrentVignette(float counter, float max)
        {
            _counter = counter;
            _max = max;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = ShowCurrentVignetteCoroutine(_counter, _max);

            StartCoroutine(_coroutine);
        }

        private void ShakeVingetteUpdate()
        {
            var sin = Mathf.Sin(Time.time * _sinVignette) / _vingetteMove;

            _material.SetFloat(VINGETTE_KEY, sin);
        }

        private IEnumerator ShowCurrentVignetteCoroutine(float counter, float max)
        {
            if (!_material.HasProperty(VINGETTE_KEY))
            {
                Debug.Log($"This material doesnt has property: {VINGETTE_KEY}");

                yield break;
            }

            _material.SetFloat(VINGETTE_KEY, _sinVignette);

            _isUpdateVingette = true;
            _currentVignette = _material.GetFloat(VINGETTE_KEY);


            var range = MAX_VALUE - MIN_VALUE;
            var step = range / max;
            var newValue = step * counter;

            while(_currentVignette > newValue)
            {
                _currentVignette -= Time.deltaTime;

                _material.SetFloat(VINGETTE_KEY, _currentVignette);
                yield return null;
            }

            _sinVignette = _currentVignette;

            if (counter == 0){
                _material.SetColor("_Color", _red);
            }
            else _material.SetColor("_Color", _baseColor);

            _isUpdateVingette = false;
        }
    }
}
