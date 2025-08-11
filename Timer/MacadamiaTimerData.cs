using UnityEngine;

namespace MacadamiaNuts.Timer
{
    [System.Serializable]
    public class MacadamiaTimerData(float min, float max, float start)
    {
        public float Min => _min;
        public float Max => _max;
        public float StartTime => _startTime;

        [SerializeField] private float _min = min;
        [SerializeField] private float _max = max;
        [SerializeField] private float _startTime = start;
    }
}
