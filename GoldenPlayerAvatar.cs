using MacadamiaNuts.Valuables;
using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace MacadamiaNuts
{
    public class GoldenPlayerAvatar : MonoBehaviour
    {
        private const string MAIN_TEXTURE = "_BaseTexture";
        private const string EDGE_STEP = "_EdgeStep";
        private const string GO_NAME = "[RIG]";

        private const float MAX_GOLD = .8f;
        private const float MIN_GOLD = .2f;

        private float _counter = 5;
        private float _maxCorruption = 0;
        private float _step => (MAX_GOLD - MIN_GOLD) / _maxCorruption;
        private bool _isFullCorrupted => _maxCorruption == _counter;

        private Animator _animator;
        private PlayerAvatar _playerAvatar;
        private PhotonView _playerPhoton;

        private GoldenNutValuable _corryptioner;

        private Renderer[] _playerRenderers;

        private Shader _goldenShader;

        public void Initialize(PlayerAvatar player, GoldenNutValuable corryptioner)
        {
            _corryptioner = corryptioner;

            _playerAvatar = player;
            _animator = _playerAvatar.transform.parent.GetComponent<Animator>();
            _playerPhoton = _playerAvatar.GetComponent<PhotonView>();
            _playerRenderers = _playerAvatar.transform.parent.Find(GO_NAME).GetComponentsInChildren<Renderer>();

            _playerPhoton.ObservedComponents.Add(this);
            _goldenShader = _corryptioner.GetComponentInChildren<Renderer>().material.shader;

            _counter = Random.Range(2, 7);

            foreach (Renderer renderer in _playerRenderers)
            {
                try
                {
                    var texture = renderer.material.GetTexture("_MainTex");
                    print(texture);
                    var goldMaterial = new Material(_goldenShader);
                    print(goldMaterial);
                    goldMaterial.SetTexture(MAIN_TEXTURE, texture);
                    goldMaterial.SetFloat(EDGE_STEP, 1);

                    renderer.material = goldMaterial;
                    print(renderer.material);

                } catch(System.Exception ex)
                {
                    Debug.LogWarning(ex.Message);
                }
                
            }
        }

        public void IncreaseCorryption()
        {
            if (_isFullCorrupted)
            {
                Death();

                return;
            }

            _counter--;

            StartCoroutine(UpdateCorryption(_counter));
        }

        public void Revive()
        {
            // it aint my fault he ran right in front of my truck
            // where, on the interstate?
            _playerPhoton.ObservedComponents.Remove(this);

            Destroy(this);
        }

        private void Death()
        {
            StartCoroutine(UpdateCorryption(0));
            _animator.speed = 0f;
            _corryptioner.RemoveCorryptedPlayer(_playerAvatar);

            _playerAvatar.playerHealth.Death();
        }

        private IEnumerator UpdateCorryption(float counter)
        {
            foreach (Renderer renderer in _playerRenderers)
            {
                var material = renderer.material;

                var currentCorryption = material.GetFloat(MAIN_TEXTURE);

                while (currentCorryption >= counter * _step)
                {
                    material.SetFloat(MAIN_TEXTURE, Time.deltaTime);
                    yield return null;
                }
            }
        }
    }
}
