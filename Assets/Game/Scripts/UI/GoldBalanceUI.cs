using System.Collections;
using Events;
using TMPro;
using UnityEngine;

namespace UI
{
  public class GoldBalanceUI : MonoBehaviour
  {
    private TMP_Text _text;
    
    private void Awake()
    {
      _text = GetComponent<TMP_Text>();
    }
    
    private void OnEnable()
    {
      if (G.GoldManager == null) 
        return;
      
      _text.text = "Gold: " + G.GoldManager.GoldBalance;
      G.EventManager.Register<OnGoldBalanceChange>(UpdateGoldBalance);
    }

    private IEnumerator Start()
    {
      yield return null;
      _text.text = "Gold: " + G.GoldManager.GoldBalance;
      G.EventManager.Register<OnGoldBalanceChange>(UpdateGoldBalance);
    }
    
    private void OnDisable()
    {
      G.EventManager.Unregister<OnGoldBalanceChange>(UpdateGoldBalance);
    }
    
    private void UpdateGoldBalance(OnGoldBalanceChange e)
    {
      _text.text = "Gold: " + e.NewBalance;
    }
  }
}