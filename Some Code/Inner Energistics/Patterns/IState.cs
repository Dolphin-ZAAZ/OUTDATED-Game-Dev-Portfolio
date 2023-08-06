namespace Energistics.Behaviour
{ 
    public interface IState
    {
        void Tick();
        void OnEnter();
        void OnExit();
    }
}