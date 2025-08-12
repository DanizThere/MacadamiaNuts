using UnityEngine;

namespace MacadamiaNuts.Timer
{
    [CreateAssetMenu(fileName = "Timer - _____", menuName = "Macadamia Nuts/Timer Data")]
    public class MacadamiaTimerData : ScriptableObject
    {
        public float Min;
        public float Max;
        public float StartTime;
    }
}
