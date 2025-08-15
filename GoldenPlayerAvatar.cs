using System.Collections;
using UnityEngine;

namespace MacadamiaNuts
{
    public class GoldenPlayerAvatar : MonoBehaviour
    {
        private const string MAIN_TEXTURE = "_MainTexture";

        private Material _goldShader;

        private int _counter = 0;
        private int _maxCorruption = 5;
        private bool _isFullCorrupted => _maxCorruption == _counter;

        private Animator _animator;

        public void Initialize()
        {
            _animator = GetComponent<Animator>();
        }

        public void IncreaseCorryption()
        {
            if (_isFullCorrupted)
            {

            }

            _counter++;
        }
    }
}
