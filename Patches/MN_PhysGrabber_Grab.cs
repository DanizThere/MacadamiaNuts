using HarmonyLib;
using MacadamiaNuts.Valuables;
using UnityEngine;

namespace MacadamiaNuts.Patches
{
    [HarmonyPatch(typeof(PhysGrabber))]
    public class MN_PhysGrabber_Grab
    {
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch(nameof(PhysGrabber.PhysGrabPointActivate))]
        [HarmonyPostfix]
        static void Postfix(PhysGrabber __instance)
        {
            if (__instance.grabbedPhysGrabObject is null) return;

            if(__instance.grabbedPhysGrabObject.TryGetComponent<GoldenNutValuable>(out var goldenNuts))
            {
                Debug.Log("you have been corrypted");

                var avatar = __instance.playerAvatar;

                goldenNuts.StartCorrypt(avatar);
            }
        }
    }
}
