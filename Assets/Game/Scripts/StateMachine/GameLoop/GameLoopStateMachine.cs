using Game.Scripts.Events;

namespace Game.Scripts.StateMachine.GameLoop
{
  public class GameLoopStateMachine : StateMachine
  {
    public enum GameLoopState
    {
      Tutorial,
      Shopping,
      Mining,
      Descend,
      Ascend
    }
    
    private readonly ShoppingState _shoppingState = new();
    private readonly MiningState _miningState = new();
    private readonly DescendState _descendState = new();
    private readonly AscendState _ascendState = new();

    public void SetState(GameLoopState newState)
    {
      switch (newState)
      {
        case GameLoopState.Shopping:
          ChangeState(_shoppingState);
          break;
        case GameLoopState.Mining:
          ChangeState(_miningState);
          break;
        case GameLoopState.Descend:
          ChangeState(_descendState);
          break;
        case GameLoopState.Ascend:
          ChangeState(_ascendState);
          break;
      }
      
      G.EventManager.Trigger(new OnGameStateChangedEvent
      {
        State = newState
      });
    }
  }
}