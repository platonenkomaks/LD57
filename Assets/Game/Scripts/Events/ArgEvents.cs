using Game.Scripts.StateMachine.GameLoop;

namespace Game.Scripts.Events
{
  public struct GameStateChangedEvent : IEvent
  {
    public GameLoopStateMachine.GameLoopState State;
  }
}