using System;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;
using UnityEngine.Events;
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
    
    /// <summary>
    /// How much gold does the player have.
    /// </summary>
    public int GoldBalance { get; private set; }

    public readonly UnityEvent<int, int> OnGoldProgressEvent = new();

    private int _checkpointGoldBalance;

    private void Awake()
    {
      G.GoldManager = this;
    }

    private void Start()
    {
      G.EventManager.Register<OnCheckpoint>(OnCheckpoint);
      G.EventManager.Register<OnPlayerRespawn>(OnPlayerRespawn);
    }

    private void OnDestroy()
    {
      G.EventManager.Unregister<OnCheckpoint>(OnCheckpoint);
      G.EventManager.Unregister<OnPlayerRespawn>(OnPlayerRespawn);
      G.GoldManager = null;
    }
    
    private void OnCheckpoint(OnCheckpoint _)
    {
      _checkpointGoldBalance = GoldBalance;
    }
    
    private void OnPlayerRespawn(OnPlayerRespawn _)
    {
      GoldBalance = _checkpointGoldBalance;
      G.EventManager.Trigger(new OnGoldBalanceChange { NewBalance = GoldBalance });
    }

    public void AddGold(int amount)
    {
      GoldBalance += amount;
      
      G.EventManager.Trigger(new OnGoldBalanceChange { NewBalance = GoldBalance });
      G.EventManager.Trigger(new OnRemainingGoldCount { RemainingGoldCount = GoldGoal });
      
      GoldGoalProgress += amount;
      
      G.ElevatorPlatform.GetComponent<PlatformWeight>().ResetWeight();
      
      OnGoldProgressEvent?.Invoke(GoldGoalProgress, GoldGoal);
      
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