using MacadamiaNuts.Valuables;
using System.Collections;
using UnityEngine;

namespace MacadamiaNuts.Golden
{
    public class GoldenPlayerAvatar : MonoBehaviour
    {
        public float Counter => _counter;
        public float MaxCorruption => _maxCorruption;

        private const string MAIN_TEXTURE = "_MainTex";
        private const string EDGE_STEP = "_GoldCorryption";

        private const float MAX_GOLD = 1f;
        private const float MIN_GOLD = 0f;

        private float _counter = 5;
        private float _maxCorruption = 0;
        private float _minCorruption = 0;
        private bool _isFullCorrupted => _minCorruption == _counter;

        private PlayerAvatar _playerAvatar;
        private PlayerAvatarVisuals _playerAvatarVisuals;

        private GoldenNutValuable _corryptioner;

        private MeshRenderer[] _playerRenderers;

        private Shader _goldenShader;

        public void Initialize(PlayerAvatar player, PlayerAvatarVisuals rig, GoldenNutValuable corryptioner)
        {
            _corryptioner = corryptioner;
            _playerAvatar = player;
            _playerAvatarVisuals = rig;
            var rigModel = _playerAvatarVisuals.meshParent;

            _goldenShader = GetComponent<Renderer>().material.shader;

            _counter = Random.Range(2, 7);
            _maxCorruption = _counter;

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

            StartCoroutine(UpdateCorryptionCoroutine(_counter));
        }

        public void Death()
        {
            StartCoroutine(UpdateCorryptionCoroutine(0));
            _corryptioner.RemoveCorryptedPlayer(_playerAvatar);

            _playerAvatar.physGrabber.ReleaseObject();
            _playerAvatar.PlayerDeath(-1);
        }

        private IEnumerator UpdateCorryptionCoroutine(float counter)
        {
            foreach (Renderer renderer in _playerRenderers)
            {
                var material = renderer.materials[^1];
                var step = (MAX_GOLD - MIN_GOLD) / _maxCorruption;
                var progress = counter * step;

                if (material.shader != _goldenShader)
                {
                    continue;
                }

                var currentCorryption = material.GetFloat(EDGE_STEP);

                var oldMaterial = renderer.materials[0];
                renderer.materials[0] = material;

                while (currentCorryption >= progress)
                {
                    currentCorryption -= Time.deltaTime;
                    material.SetFloat(EDGE_STEP, currentCorryption);
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
                else if (renderer.material.name == "Player Avatar - Eye") continue;
                else continue;

                var goldMaterial = new Material(_goldenShader)
                {
                    name = "GoldCorryption"
                };
                goldMaterial.SetTexture(MAIN_TEXTURE, texture);
                goldMaterial.SetFloat(EDGE_STEP, MAX_GOLD);

                var newMaterials = new Material[renderer.materials.Length + 1];

                renderer.materials.CopyTo(newMaterials, 0);
                newMaterials[^1] = goldMaterial;

                renderer.materials = newMaterials;
            }
        }
    }
}
