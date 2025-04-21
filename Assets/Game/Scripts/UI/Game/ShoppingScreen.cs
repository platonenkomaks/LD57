using DG.Tweening;
using UnityEngine;

namespace UI.Game
{
  public class ShoppingScreen : MonoBehaviour
  {
    [SerializeField] private CanvasGroup shopCanvasGroup;
    
    public void ShowShopScreen()
    {
      shopCanvasGroup.DOKill();
      shopCanvasGroup.alpha = 0;
      shopCanvasGroup.interactable = false;
      shopCanvasGroup.gameObject.SetActive(true);
      shopCanvasGroup
        .DOFade(1f, 0.25f)
        .OnComplete(() => shopCanvasGroup.interactable = true);
    }
    
    public void HideShopScreen()
    {
      shopCanvasGroup.DOKill();
      shopCanvasGroup.interactable = false;
      shopCanvasGroup
        .DOFade(0f, 0.25f)
        .OnComplete(() => shopCanvasGroup.gameObject.SetActive(false));
    }
  }
}