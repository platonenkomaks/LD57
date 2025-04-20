    using UnityEngine;
    using UnityEngine.EventSystems;

    public class Tooltip: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject tooltip;
        
        private void Awake()
        {
            if (tooltip == null) return;
            tooltip.SetActive(false);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
             G.AudioManager.Play("Interact");
            if (tooltip == null) return;
            ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltip == null) return;
            HideTooltip();
        }

        private void ShowTooltip()
        {
            tooltip.SetActive(true);
        }


        public void HideTooltip()
        {
            tooltip.SetActive(false);
        }
    }
    
