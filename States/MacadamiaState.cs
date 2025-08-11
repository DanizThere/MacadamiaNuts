namespace MacadamiaNuts.States
{
    public abstract class MacadamiaState
    {
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}
