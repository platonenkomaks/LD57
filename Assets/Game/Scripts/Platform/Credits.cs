using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Platform
{
    public class Credits : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer panel1;
        [SerializeField] private SpriteRenderer panel2;

        public bool isFirstCall = true;

        public void StartCredits()
        {
            if (!isFirstCall) return;
            StartCoroutine(CreditsView());
        }

        private void Awake()
        {
            SetAlpha(panel1, 0f);
            panel1.gameObject.SetActive(false);
            SetAlpha(panel2, 0f);
            panel2.gameObject.SetActive(false);
        }

        private IEnumerator CreditsView()
        {
            yield return new WaitForSeconds(1f);

            // Fade in panel1
            panel1.gameObject.SetActive(true);
            panel1.DOFade(1f, 2f);
            yield return new WaitForSeconds(4f);

            // Fade out panel1
            panel1.DOFade(0f, 1f).OnComplete(() => panel1.gameObject.SetActive(false));

            // Fade in panel2
            yield return new WaitForSeconds(1f);
            panel2.gameObject.SetActive(true);
            panel2.DOFade(1f, 1f);
            yield return new WaitForSeconds(6f);

            // Fade out panel2
            panel2.DOFade(0f, 1f).OnComplete(() => panel2.gameObject.SetActive(false));

            isFirstCall = false;
        }

        private void SetAlpha(SpriteRenderer spriteRenderer, float alpha)
        {
            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    public static class SpriteRendererExtensions
    {
        public static Tweener DOFade(this SpriteRenderer spriteRenderer, float endValue, float duration)
        {
            return DOTween.ToAlpha(
                () => spriteRenderer.color,
                x => spriteRenderer.color = x,
                endValue,
                duration
            );
        }
    }
}