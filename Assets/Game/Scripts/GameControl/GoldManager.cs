using Game.Scripts.Events;
using UnityEngine;

namespace GameControl
{
  public class GoldManager : MonoBehaviour
  {
    /// <summary>
    /// How much gold is available in the cave for mining.
    /// </summary>
    public int GoldRemaining { get; private set; }
    
    /// <summary>
    /// How much gold does the player have.
    /// </summary>
    public int GoldBalance { get; private set; }


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
      GoldRemaining = gold;
      G.EventManager.Trigger(new OnRemainingGoldCount { RemainingGoldCount = GoldRemaining });
    }

    public void AddGold(int amount)
    {
      GoldBalance += amount;
      GoldRemaining -= amount;
      
      G.EventManager.Trigger(new OnGoldBalanceChange { NewBalance = GoldBalance });
      G.EventManager.Trigger(new OnRemainingGoldCount { RemainingGoldCount = GoldRemaining });
    }

    public bool CanAfford(int amount) => GoldBalance >= amount;

    public void ConsumeGold(int amount)
    {
      GoldBalance -= amount;
      G.EventManager.Trigger(new OnGoldBalanceChange { NewBalance = GoldBalance });
    }
  }
}