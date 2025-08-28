using HarmonyLib;
using MacadamiaNuts.Golden;
using UnityEngine;

namespace MacadamiaNuts.Patches
{
    [HarmonyPatch(typeof(ExtractionPoint))]
    public class MN_ExtractsionPoint_DestroyAllPhysObjectsInHaulList
    {
        [HarmonyPatch(nameof(ExtractionPoint.DestroyAllPhysObjectsInHaulList))]
        [HarmonyPrefix]
        static bool Prefix(ExtractionPoint __instance)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer())
            {
                foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
                {
                    var goldenHead = player.transform.parent.GetComponentInChildren<GoldenHead>();
                    if (goldenHead is null) return true;

                    RoundDirector.instance.totalHaul += (int)goldenHead.PlayerCost;

                    player.playerDeathHead.Revive();
                    goldenHead.Revive();
                }
                foreach (GameObject dollarHaul in RoundDirector.instance.dollarHaulList)
                {
                    if ((bool)dollarHaul && (bool)dollarHaul.GetComponent<PhysGrabObject>())
                    {
                        RoundDirector.instance.totalHaul += (int)dollarHaul.GetComponent<ValuableObject>().dollarValueCurrent;
                        dollarHaul.GetComponent<PhysGrabObject>().DestroyPhysGrabObject();
                    }
                }
            }

            return false;
        }
    }
}
