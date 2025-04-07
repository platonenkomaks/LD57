using Events;
using TMPro;
using UnityEngine;

namespace UI
{
  public class GameUI : MonoBehaviour
  {
    [SerializeField] private TMP_Text goldRemainingText;
    [SerializeField] private TMP_Text goldBalance;

    private void Start()
    {
      G.EventManager.Register<OnRemainingGoldCount>(UpdateCaveGold);
      G.EventManager.Register<OnGoldBalanceChange>(UpdateGoldBalance);
    }

    private void OnDestroy()
    {
      G.EventManager.Unregister<OnRemainingGoldCount>(UpdateCaveGold);
      G.EventManager.Unregister<OnGoldBalanceChange>(UpdateGoldBalance);
    }

    private void UpdateCaveGold(OnRemainingGoldCount e)
    {
      goldRemainingText.text = "Gold in the Mine: " + e.RemainingGoldCount;
    }
    
    private void UpdateGoldBalance(OnGoldBalanceChange e)
    {
      goldBalance.text = "Gold: " + e.NewBalance;
    }
  }
}