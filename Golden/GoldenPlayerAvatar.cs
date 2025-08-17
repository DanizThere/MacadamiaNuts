using MacadamiaNuts.Valuables;
using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace MacadamiaNuts.Golden
{
    public class GoldenPlayerAvatar : MonoBehaviour
    {
        private const string MAIN_TEXTURE = "_MainTex";
        private const string EDGE_STEP = "_GoldCorryption";

        private const float MAX_GOLD = 1f;
        private const float MIN_GOLD = 0f;

        private float _counter = 5;
        private float _maxCorruption = 0;
        private float _step => (MAX_GOLD - MIN_GOLD) / _maxCorruption;
        private bool _isFullCorrupted => _maxCorruption == _counter;

        private Animator _animator;
        private PlayerAvatar _playerAvatar;
        private PlayerAvatarVisuals _playerAvatarVisuals;
        private PhotonView _playerPhoton;

        private GoldenNutValuable _corryptioner;

        private MeshRenderer[] _playerRenderers;

        private Shader _goldenShader;

        public void Initialize(PlayerAvatar player, PlayerAvatarVisuals rig, GoldenNutValuable corryptioner)
        {
            _corryptioner = corryptioner;
            _playerAvatar = player;
            _playerAvatarVisuals = rig;
            var rigModel = _playerAvatarVisuals.meshParent;

            _animator = _playerAvatarVisuals.GetComponent<Animator>();
            _playerPhoton = _playerAvatar.GetComponent<PhotonView>();

            _playerPhoton.ObservedComponents.Add(this);
            _goldenShader = Shader.Find("MacadamiaNuts/GoldShader");

            _counter = Random.Range(2, 7);

            _playerRenderers = rigModel.transform.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
            AddMaterial();
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

            _playerAvatar.physGrabber.ReleaseObject();

            _playerAvatar.SetSpectate();

            //if(_playerAvatar.playerDeathHead.TryGetComponent<GoldenHead>(out var head))
            //{

            //}
            //else
            //{
            //    MacadamiaNuts.Instance.GoldenHeadFactory.Create(_playerAvatar.playerDeathHead.transform);
            //}
        }

        private IEnumerator UpdateCorryption(float counter)
        {
            foreach (Renderer renderer in _playerRenderers)
            {
                var material = renderer.materials.FirstOrDefault(x => x.name == "GoldCorryption");

                if (!material)
                {
                    Debug.Log("нету материала");
                    continue;
                }

                var currentCorryption = material.GetFloat(EDGE_STEP);

                var oldMaterial = renderer.materials[0];
                renderer.materials[0] = material;
                while (currentCorryption >= counter * _step)
                {
                    material.SetFloat(EDGE_STEP, Time.deltaTime);
                    yield return null;
                }

                renderer.materials[0] = oldMaterial;
            }
        }

        private void AddMaterial()
        {
            foreach (Renderer renderer in _playerRenderers)
            {
                Texture texture;
                if (renderer.material.HasProperty("_AlbedoTexture"))
                {
                    texture = renderer.material.GetTexture("_AlbedoTexture");
                }
                else if (renderer.material.HasProperty("_MainTex"))
                {
                    texture = renderer.material.mainTexture;
                }
                else continue;

                var goldMaterial = new Material(_goldenShader)
                {
                    name = "GoldCorryption"
                };
                goldMaterial.SetTexture(MAIN_TEXTURE, texture);
                goldMaterial.SetFloat(EDGE_STEP, 1);

                var newMaterials = new Material[renderer.materials.Length + 1];
                newMaterials[newMaterials.Length] = goldMaterial;

                renderer.materials = newMaterials;
            }
        }
    }
}
