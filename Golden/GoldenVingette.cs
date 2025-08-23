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

        private Image _vingette;

        private float _currentVignette;

        private Material _materialPrefab;
        private Material _material;

        private void Awake()
        {
            _vingette = GetComponentInChildren<Image>();
            _materialPrefab = _vingette.material;

            _material = new(_materialPrefab);
            _vingette.material = _material;

            if (!_material.HasProperty(VINGETTE_KEY))
            {
                Debug.Log($"This material doesnt has property: {VINGETTE_KEY}");

                return;
            }

            _material.SetFloat(VINGETTE_KEY, MAX_VALUE);

            Hide();
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
            StartCoroutine(ShowCurrentVignetteCoroutine(counter, max));
        }

        private IEnumerator ShowCurrentVignetteCoroutine(float counter, float max)
        {
            if (!_material.HasProperty(VINGETTE_KEY))
            {
                Debug.Log($"This material doesnt has property: {VINGETTE_KEY}");

                yield break;
            }

            var step = (MAX_VALUE - MIN_VALUE) / max;
            var newValue = step * counter;

            _currentVignette = _material.GetFloat(VINGETTE_KEY);

            while(_currentVignette > newValue)
            {
                _currentVignette -= Time.deltaTime;

                _material.SetFloat(VINGETTE_KEY, _currentVignette);
                yield return null;
            }
        }
    }
}
