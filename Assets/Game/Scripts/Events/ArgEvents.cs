using Game.Scripts.StateMachine.GameLoop;

namespace Events
{
  public struct OnGameStateChangedEvent : IEvent
  {
    public GameLoopStateMachine.GameLoopState State;
  }
  
  public struct SetGameStateEvent : IEvent
  {
    public GameLoopStateMachine.GameLoopState State;
  }

  public struct OnRemainingGoldCount : IEvent
  {
    public int RemainingGoldCount;
  }

  public struct OnGoldBalanceChange : IEvent
  {
    public int NewBalance;
  }
  
  public struct OnBatteryLightChargeChange : IEvent
  {
    public float BatteryCharge;
  }

  public struct OnPlatformEnter : IEvent {}
  
  public struct OnPlatformExit : IEvent {}
  
  public struct OnPlayerStateChangeEvent : IEvent
  {
    public PlayerStateMachine.PlayerState State;
  }
}