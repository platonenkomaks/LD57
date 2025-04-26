using DG.Tweening;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UI.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
  public class GameUI : MonoBehaviour
  {
    [SerializeField] private Image foregroundTint;
    [SerializeField] private GameObject shopButton;
    [SerializeField] private ShoppingScreen shoppingScreen;

    private void Start()
    {
      G.EventManager.Register<OnPlayerDeath>(OnPlayerDeath);
      G.EventManager.Register<OnPlayerRespawn>(OnPlayerRespawn);
      G.EventManager.Register<OnGameStateChangedEvent>(OnGameStateChanged);
      DoFadeOut();
    }

    private void OnDestroy()
    {
      G.EventManager.Unregister<OnPlayerDeath>(OnPlayerDeath);
      G.EventManager.Unregister<OnPlayerRespawn>(OnPlayerRespawn);
      G.EventManager.Unregister<OnGameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnPlayerDeath(OnPlayerDeath e)
    {
      DoFadeIn(() => G.EventManager.Trigger(new OnPlayerRespawn()));
    }

    private void OnPlayerRespawn(OnPlayerRespawn e)
    {
      DoFadeOut(delay: 0.5f);
    }
    
    private void OnGameStateChanged(OnGameStateChangedEvent e)
    {
      bool isShopping = e.State == GameLoopStateMachine.GameLoopState.Shopping;
      shopButton.SetActive(isShopping);

      if (isShopping)
      {
        shoppingScreen.ShowShopScreen(4f);
      }
    }

    public void Restart()
    {
      foregroundTint.color = new Color(0, 0, 0, 0);
      foregroundTint.gameObject.SetActive(true);
      
      Sequence seq = DOTween.Sequence();
      seq.Append(foregroundTint.DOFade(1f, 0.5f));
      seq.AppendCallback(() =>
      {
        G.EventManager.Trigger(new OnGameStateChangedEvent { State = GameLoopStateMachine.GameLoopState.Tutorial});
        G.SceneLoader.LoadScene("Game");
      });
    }
    
    private void DoFadeIn(UnityAction onComplete = null)
    {
      foregroundTint.color = new Color(0, 0, 0, 0);
      foregroundTint.gameObject.SetActive(true);

      Sequence seq = DOTween.Sequence();
      seq.Append(foregroundTint.DOFade(1f, 0.5f));
      seq.OnComplete(() => onComplete?.Invoke());
    }

    private void DoFadeOut(UnityAction onComplete = null, float delay = 0)
    {
      foregroundTint.color = new Color(0, 0, 0, 1);
      foregroundTint.gameObject.SetActive(true);
      
      Sequence seq = DOTween.Sequence();
      seq.Append(foregroundTint.DOFade(0f, 0.5f));
      seq.SetDelay(delay);
      seq.OnComplete(() =>
      {
        foregroundTint.gameObject.SetActive(false);
        onComplete?.Invoke();
      });
    }
  }
}