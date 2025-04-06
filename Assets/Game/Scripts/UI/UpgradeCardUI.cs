using DG.Tweening;
using Stats.BaseClasses;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
  public class UpgradeCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text upgradeInfoText;
    [SerializeField] private TMP_Text costText;

    private StatSOBase _upgradeStat;
    private int _upgradeIndex;
    
    public void Initialize(StatSOBase upgradeStat, int upgradeIndex)
    {
      _upgradeStat = upgradeStat;
      _upgradeIndex = upgradeIndex;
      
      _upgradeStat.OnUpgrade.AddListener(UpdateButton);
      upgradeButton.onClick.AddListener(_upgradeStat.ApplyNextUpgrade);
      
      IntStat intStat = _upgradeStat as IntStat;
      if (intStat != null)
      {
        upgradeInfoText.text = intStat.Upgrades[_upgradeIndex].ToString();
        return;
      }
      
      FloatStat floatStat = _upgradeStat as FloatStat;
      if (floatStat != null)
      {
        upgradeInfoText.text = floatStat.Upgrades[_upgradeIndex].ToString("F1");
        return;
      }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
      transform.DOScale(Vector3.one * 1.1f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      transform.DOScale(Vector3.one, 0.2f);
    }

    private void UpdateButton()
    {
      int nextUpgradeIndex = _upgradeStat.NextUpgradeIndex;
      
      upgradeButton.interactable = nextUpgradeIndex == _upgradeIndex;
        
      var block = upgradeButton.colors;
      block.disabledColor = _upgradeIndex < nextUpgradeIndex ? Color.green : Color.gray;
      upgradeButton.colors = block;
    }
  }
}