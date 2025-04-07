using System.Collections.Generic;
using Stats.BaseClasses;
using TMPro;
using UnityEngine;


namespace UI
{
  public class UpgradeUI : MonoBehaviour
  {
    [SerializeField] private StatSOBase upgradeStat;
    [SerializeField] private List<UpgradeCardUI> upgradeButtons;
    [SerializeField] private List<Sprite> upgradeIconsUnpaid;
    [SerializeField] private List<Sprite> upgradeIconsPaid;
    [SerializeField] private TMP_Text statName;
    
    private void Awake()
    {
      statName.text = upgradeStat.StatName;
      
      for (int i = 0; i < upgradeButtons.Count; i++)
      {
        upgradeButtons[i].Initialize(upgradeStat, i, upgradeIconsUnpaid[i], upgradeIconsPaid[i]);
      }
    }
  }
}