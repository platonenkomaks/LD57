using System;
using Events;
using UnityEngine;
using UnityEngine.Events;

namespace GameControl
{
  public class GoldManager : MonoBehaviour
  {
    /// <summary>
    /// How much gold is available in the cave for mining.
    /// </summary>
    public int GoldGoal { get; private set; }
    
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

    public void SetGoldRemaining(int gold)
    {
      GoldGoal = gold;
      G.EventManager.Trigger(new OnRemainingGoldCount { RemainingGoldCount = GoldGoal });
    }

    public void AddGold(int amount)
    {
      
      GoldBalance += amount;
      GoldGoal -= amount;
      
      G.EventManager.Trigger(new OnGoldBalanceChange { NewBalance = GoldBalance });
      G.EventManager.Trigger(new OnRemainingGoldCount { RemainingGoldCount = GoldGoal });
      OnGoldBalanceChange?.Invoke();
    }

    public bool CanAfford(int amount) => GoldBalance >= amount;

    public void ConsumeGold(int amount)
    {
      GoldBalance -= amount;
      G.EventManager.Trigger(new OnGoldBalanceChange { NewBalance = GoldBalance });
    }
  }
}