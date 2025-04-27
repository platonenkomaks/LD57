using Events;

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
      Ascend,
      Win
    }
    
    private readonly ShoppingState _shoppingState = new();
    private readonly MiningState _miningState = new();
    private readonly DescendState _descendState = new();
    private readonly AscendState _ascendState = new();

    public GameLoopStateMachine()
    {
      G.EventManager.Register<OnPlayerRespawn>(OnPlayerRespawn);
    }
    
    private void OnDestroy()
    {
      G.EventManager.Unregister<OnPlayerRespawn>(OnPlayerRespawn);
    }

    public void SetState(GameLoopState newState)
    {
      switch (newState)
      {
        case GameLoopState.Shopping:
          ChangeState(_shoppingState);
          G.Player.BatteryLight.FullRecharge();
          break;
        case GameLoopState.Mining:
          ChangeState(_miningState);
          G.Player.BatteryLight.FullRecharge();
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

    private void OnPlayerRespawn(OnPlayerRespawn _)
    {
      this.SetState(GameLoopState.Shopping);
    }
  }
}