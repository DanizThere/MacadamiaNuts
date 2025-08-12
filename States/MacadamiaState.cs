namespace MacadamiaNuts.States
{
    public abstract class MacadamiaState
    {
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
