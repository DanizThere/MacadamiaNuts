using HarmonyLib;
using MacadamiaNuts.Golden;
using MacadamiaNuts.Valuables;

namespace MacadamiaNuts.Patches
{
    [HarmonyPatch(typeof(PhysGrabber))]
    public class MN_PhysGrabber_Release
    {
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch(nameof(PhysGrabber.PhysGrabPointDeactivate))]
        [HarmonyPostfix]
        static void Postfix(PhysGrabber __instance)
        {
            if (__instance.grabbedPhysGrabObject is null) return;

            if (__instance.grabbedPhysGrabObject.TryGetComponent<GoldenNutValuable>(out var _))
            {
                var avatar = __instance.playerAvatar;

                avatar.transform.parent.GetComponentInChildren<GoldenHead>().ResetGrabbing();
            }
        }
    }
}
