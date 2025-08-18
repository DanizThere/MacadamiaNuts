using UnityEngine;

namespace MacadamiaNuts.States
{
    public abstract class MacadamiaState : MonoBehaviour
    {
        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
