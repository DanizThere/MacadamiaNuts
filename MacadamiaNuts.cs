using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using REPOLib;
using REPOLib.Commands;


namespace MacadamiaNuts
{
    [BepInPlugin("DanizHereAndNow.MacadamiaNuts", "MacadamiaNuts", "1.0")]
    [BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class MacadamiaNuts : BaseUnityPlugin
    {
        internal static MacadamiaNuts Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger => Instance._logger;
        private ManualLogSource _logger => base.Logger;
        internal Harmony? Harmony { get; set; }
        internal NutsValuable? NutsValuableInstance { get; private set; }

        private void Awake()
        {
            Instance = this;

            // Prevent the plugin from being deleted
            this.gameObject.transform.parent = null;
            this.gameObject.hideFlags = HideFlags.HideAndDontSave;

            //NutsValuableInstance = this.gameObject.AddComponent<NutsValuable>();

            Patch();

            Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        }

        internal void Patch()
        {
            Harmony ??= new Harmony(Info.Metadata.GUID);
            Harmony.PatchAll();
        }

        internal void Unpatch()
        {
            Harmony?.UnpatchSelf();
        }

        private void Update()
        {
            
        }
    }
}