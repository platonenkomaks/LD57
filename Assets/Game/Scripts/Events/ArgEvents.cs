using Game.Scripts.StateMachine.GameLoop;

namespace Game.Scripts.Events
{
  public struct OnGameStateChangedEvent : IEvent
  {
    public GameLoopStateMachine.GameLoopState State;
  }
  
  public struct SetGameStateEvent : IEvent
  {
    public GameLoopStateMachine.GameLoopState State;
  }
}