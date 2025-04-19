using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
  public class PointerScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    [SerializeField] private float scaleFactor = 1.5f;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
      transform.DOScale(Vector3.one * scaleFactor, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      transform.DOScale(Vector3.one, 0.2f);
    }
  }
}