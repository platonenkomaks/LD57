using DG.Tweening;
using UnityEngine;
    public class HUD : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        
        private void Awake ()
        {
            G.HUD = this;
            canvasGroup.alpha = 0;
        }

        private void OnDestroy()
        {
            G.HUD = null;
        }

        public void Show()
        {
            canvasGroup.DOFade(1, 1f);
        }
        public void Hide()
        {
            canvasGroup.DOFade(0, 1f);
        }
    }
