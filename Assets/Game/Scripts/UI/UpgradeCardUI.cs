using Game.Scripts.Events;
using Stats.BaseClasses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class UpgradeCardUI : MonoBehaviour
  {
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text upgradeInfoText;
    [SerializeField] private TMP_Text costText;

    private StatSOBase _upgradeStat;
    private IntStat _intStat;
    private FloatStat _floatStat;
    private int _upgradeIndex;
    private int _cost;
    
    public void Initialize(StatSOBase upgradeStat, int upgradeIndex)
    {
      _upgradeStat = upgradeStat;
      _upgradeIndex = upgradeIndex;
      
      _upgradeStat.OnUpgrade.AddListener(UpdateButton);
      upgradeButton.onClick.AddListener(_upgradeStat.ApplyNextUpgrade);
      
      _intStat = _upgradeStat as IntStat;
      if (_intStat != null)
      {
        upgradeInfoText.text = _intStat.Upgrades[_upgradeIndex].ToString();
        _cost = _intStat.UpgradeCosts[_upgradeIndex];
        costText.text = _cost + " gold";
        return;
      }
      
      _floatStat = _upgradeStat as FloatStat;
      if (_floatStat != null)
      {
        upgradeInfoText.text = _floatStat.Upgrades[_upgradeIndex].ToString("F1");
        _cost = _floatStat.UpgradeCosts[_upgradeIndex];
        costText.text = _cost + " gold";
        return;
      }
    }

    private void OnEnable()
    {
      G.EventManager.Register<OnGoldBalanceChange>(OnGoldBalanceChange);
      
      if (_upgradeStat != null)
      {
        UpdateButton();
      }
    }
    
    private void OnDisable()
    {
      G.EventManager.Unregister<OnGoldBalanceChange>(OnGoldBalanceChange);
    }

    private void OnGoldBalanceChange(OnGoldBalanceChange e)
    {
      UpdateButton();
    }
    
    private void UpdateButton()
    {
      int nextUpgradeIndex = _upgradeStat.NextUpgradeIndex;
      bool canAfford = G.GoldManager.CanAfford(_cost);
      
      upgradeButton.interactable = nextUpgradeIndex == _upgradeIndex;
      
      ColorBlock block = upgradeButton.colors;
      block.highlightedColor = canAfford ? Color.green : Color.red;
      block.selectedColor = canAfford ? Color.green : Color.red;
      block.disabledColor = _upgradeIndex < nextUpgradeIndex ? Color.green : Color.gray;
      upgradeButton.colors = block;
    }
  }
}