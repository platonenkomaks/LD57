namespace Game.Scripts.StateMachine
{
  public class StateMachine
  {
    private IState _currentState;
    
    protected void ChangeState(IState newState)
    {
      _currentState?.Exit();
      _currentState = newState;
      _currentState?.Enter();
    }

    public void Update()
    {
      _currentState?.Execute();
    }
  }
}