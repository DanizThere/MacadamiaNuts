using BepInEx;
using UnityEngine;

namespace MacadamiaNuts.Golden
{
    public class GoldenHeadFactory
    {
        private GameObject _prefab;

        public GoldenHeadFactory()
        {
            REPOLib.BundleLoader.LoadBundle(Paths.PluginPath, bundle =>
            {
                _prefab = bundle.LoadAsset<GameObject>("GoldenHeadPrefab");

                if(_prefab is null)
                {
                    Debug.Log("_prefab is null!");
                }
            });
        }

        public GoldenHead Create<T>(T parent) where T : Transform
        {
            var go = Object.Instantiate(_prefab, parent);
            var head = go.GetComponent<GoldenHead>();

            head.Initialize();

            return head;
        }
    }
}
