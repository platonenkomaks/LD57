using System.Collections.Generic;
using Stats.BaseClasses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class UpgradeUI : MonoBehaviour
  {
    [SerializeField] private StatSOBase upgradeStat;
    [SerializeField] private List<Button> upgradeButtons;
    [SerializeField] private TMP_Text statName;
    
    private void Awake()
    {
      upgradeStat.OnUpgrade.AddListener(OnStatUpgrade);
      
      InitButtons();
      statName.text = upgradeStat.StatName;
    }

    private void OnStatUpgrade()
    {
      UpdateButtons();
    }

    private void InitButtons()
    {
      foreach (var button in upgradeButtons)
      {
        button.onClick.AddListener(() => upgradeStat.ApplyNextUpgrade());
      }
      
      IntStat intStat = upgradeStat as IntStat;
      if (intStat != null)
      {
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
          Button button = upgradeButtons[i];
          button.GetComponentInChildren<TMP_Text>().text = intStat.Upgrades[i].ToString();
        }

        return;
      }
      
      FloatStat floatStat = upgradeStat as FloatStat;
      if (floatStat != null)
      {
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
          Button button = upgradeButtons[i];
          button.GetComponentInChildren<TMP_Text>().text = floatStat.Upgrades[i].ToString("F1");
        }

        return;
      }
    }

    private void UpdateButtons()
    {
      int nextUpgradeIndex = upgradeStat.NextUpgradeIndex;
      for (int i = 0; i < upgradeButtons.Count; i++)
      {
        Button button = upgradeButtons[i];
        button.interactable = nextUpgradeIndex == i;
        
        var block = button.colors;
        block.disabledColor = i < nextUpgradeIndex ? Color.green : Color.gray;
        button.colors = block;
      }
    }
  }
}