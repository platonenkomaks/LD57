using System;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;
using Utilities;

namespace GameControl
{
  public class GoldManager : MonoBehaviour
  {
    /// <summary>
    /// How much gold is needed for the Win.
    /// </summary>
    public int GoldGoal { get; private set; } = 30;

    /// <summary>
    /// How much gold has been collected.
    /// </summary>
    public int GoldGoalProgress { get; private set; }

    [HideInInspector]
    public Observable<float> goldGoalProgress01 = new(0f);
    
    /// <summary>
    /// How much gold does the player have.
    /// </summary>
    public int GoldBalance { get; private set; }

    public Action OnGoldBalanceChange;

    private void Awake()
    {
      G.GoldManager = this;
    }
    
    private void OnDestroy()
    {
      G.GoldManager = null;
    }

    public void AddGold(int amount)
    {
      GoldBalance += amount;
      
      G.EventManager.Trigger(new OnGoldBalanceChange { NewBalance = GoldBalance });
      G.EventManager.Trigger(new OnRemainingGoldCount { RemainingGoldCount = GoldGoal });
      
      GoldGoalProgress++;
      goldGoalProgress01.Value = (float)GoldGoalProgress / GoldGoal;
      G.ElevatorPlatform.GetComponent<PlatformWeight>().ResetWeight();
      
      OnGoldBalanceChange?.Invoke();
      
      
      if (GoldGoalProgress >= GoldGoal)
      {
        G.EventManager.Trigger(new OnGameStateChangedEvent { State = GameLoopStateMachine.GameLoopState.Win });
      }
    }

    public bool CanAfford(int amount) => GoldBalance >= amount;

    public void ConsumeGold(int amount)
    {
      GoldBalance -= amount;
      G.EventManager.Trigger(new OnGoldBalanceChange { NewBalance = GoldBalance });
    }
  }
}