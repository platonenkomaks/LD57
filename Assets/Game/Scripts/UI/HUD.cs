using DG.Tweening;
using UnityEngine;
    public class HUD : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        
        private void Awake ()
        {
            G.HUD = this;
        }

        private void OnDestroy()
        {
            G.HUD = null;
        }

        public void Show()
        {
            canvasGroup.alpha = 0;
            gameObject.SetActive(true);
            canvasGroup.DOFade(1, 1f);
        }
    }
