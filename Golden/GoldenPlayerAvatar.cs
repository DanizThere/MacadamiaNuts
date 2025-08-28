using MacadamiaNuts.Valuables;
using System.Collections;
using UnityEngine;

namespace MacadamiaNuts.Golden
{
    public class GoldenPlayerAvatar : MonoBehaviour
    {
        public float Counter => _counter;
        public float MaxCorruption => _maxCorruption;
        public float CostValue => _costValue;

        private const string MAIN_TEXTURE = "_MainTex";
        private const string EDGE_STEP = "_GoldCorryption";

        private const float MAX_GOLD = 1f;
        private const float MIN_GOLD = 0f;

        private float _counter = 0;
        private float _maxCorruption = 0;
        private bool _isFullCorrupted => 0 == _counter;

        private float _costValue;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private PlayerAvatar _playerAvatar;
        private PlayerAvatarVisuals _playerAvatarVisuals;
        private GoldenNutValuable _corryptioner;

        private MeshRenderer[] _playerRenderers;

        private Shader _goldenShader;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.


        public void Initialize(PlayerAvatar player, PlayerAvatarVisuals rig, GoldenNutValuable corryptioner)
        {
            _corryptioner = corryptioner;
            _playerAvatar = player;
            _playerAvatarVisuals = rig;
            var rigModel = _playerAvatarVisuals.meshParent;

            _goldenShader = GetComponent<Renderer>().material.shader;

            _maxCorruption = Random.Range(2, 7);
            _counter = _maxCorruption;

            _playerRenderers = rigModel.transform.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
            AddMaterial();
        }

        public void UpdateCorryptioner(GoldenNutValuable corryptioner)
        {
            if (GoldenNutValuable.Equals(corryptioner, _corryptioner)) return;

            _corryptioner = corryptioner;
            _costValue = corryptioner.GetComponent<ValuableObject>().dollarValueCurrent * Random.Range(.7f, 1.1f);
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
            ResetCorryption();
            _playerAvatar.physGrabber.ReleaseObject();
            _playerAvatar.PlayerDeath(-1);

            _corryptioner.OnPlayerDeath?.Invoke();
        }

        private void ResetCorryption()
        {
            foreach (var renderer in _playerRenderers)
            {
                if (renderer.materials.Length == 1 || !renderer.material.HasProperty(EDGE_STEP)) continue;
                var oldMaterial = renderer.materials[0];
                renderer.materials[0] = renderer.materials[^1];
                renderer.materials[^1] = oldMaterial;
            }
        }

        private IEnumerator UpdateCorryptionCoroutine(float counter)
        {
            foreach (var renderer in _playerRenderers)
            {
                var material = renderer.materials[^1];
                var color = Color.white;
                if (renderer.materials[0].HasProperty("_AlbedoColor"))
                {
                    color = renderer.materials[0].GetColor("_AlbedoColor");
                }

                var step = (MAX_GOLD - MIN_GOLD) / _maxCorruption;
                var progress = counter * step;

                if (material.shader != _goldenShader)
                {
                    continue;
                }

                var currentCorryption = material.GetFloat(EDGE_STEP);
                material.SetColor("_BaseColor", color);

                var oldMaterial = renderer.materials[0];
                print(oldMaterial);
                print(material);
                renderer.materials[0] = material;
                renderer.materials[^1] = oldMaterial;

                while (currentCorryption >= progress)
                {
                    currentCorryption -= Time.deltaTime;
                    material.SetFloat(EDGE_STEP, currentCorryption);
                    yield return null;
                }

                renderer.materials[0] = oldMaterial;
            }

            yield return new WaitForSeconds(1f);

            foreach(var renderer in _playerRenderers)
            {
                var oldMaterial = renderer.materials[0];
                renderer.materials[0] = renderer.materials[^1];
                renderer.materials[^1] = oldMaterial;
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
