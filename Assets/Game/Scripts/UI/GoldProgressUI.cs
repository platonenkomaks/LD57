using System.Collections;
using Events;
using TMPro;
using UnityEngine;

namespace UI
{
  public class GoldProgressUI : MonoBehaviour
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
      
      _text.text = "Gold in the Mine: " + G.GoldManager.GoldGoal;
      G.EventManager.Register<OnRemainingGoldCount>(UpdateGoldBalance);
    }

    private IEnumerator Start()
    {
      yield return null;
      _text.text = "Gold in the Mine: " + G.GoldManager.GoldGoal;
      G.EventManager.Register<OnRemainingGoldCount>(UpdateGoldBalance);
    }
    
    private void OnDisable()
    {
      G.EventManager.Unregister<OnRemainingGoldCount>(UpdateGoldBalance);
    }
    
    private void UpdateGoldBalance(OnRemainingGoldCount e)
    {
      _text.text = "Gold in the Mine: " + e.RemainingGoldCount;
    }
  }
}