using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
  public class PointerScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    public void OnPointerEnter(PointerEventData eventData)
    {
      transform.DOScale(Vector3.one * 1.1f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      transform.DOScale(Vector3.one, 0.2f);
    }
  }
}