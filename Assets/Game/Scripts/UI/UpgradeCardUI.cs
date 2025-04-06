using Stats.BaseClasses;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
  public class UpgradeCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    [SerializeField] private Button upgradeButton;

    public void Initialize(StatSOBase stat)
    {
      
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
      throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      throw new System.NotImplementedException();
    }
  }
}