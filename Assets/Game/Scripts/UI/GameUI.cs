using DG.Tweening;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UI.Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class GameUI : MonoBehaviour
  {
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Image foregroundTint;
    [SerializeField] private GameObject shopButton;
    [SerializeField] private ShoppingScreen shoppingScreen;

    private void Start()
    {
      G.EventManager.Register<OnPlayerDeath>(OnGameOver);
      G.EventManager.Register<OnGameStateChangedEvent>(OnGameStateChanged);
      DoFadeOut();
    }

    private void OnDestroy()
    {
      G.EventManager.Unregister<OnPlayerDeath>(OnGameOver);
      G.EventManager.Unregister<OnGameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnGameOver(OnPlayerDeath e)
    {
      _gameOverPanel.SetActive(true);
    }
    
    private void OnGameStateChanged(OnGameStateChangedEvent e)
    {
      bool isShopping = e.State == GameLoopStateMachine.GameLoopState.Shopping;
      shopButton.SetActive(isShopping);

      if (isShopping)
      {
        shoppingScreen.ShowShopScreen();
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

    private void DoFadeOut()
    {
      foregroundTint.color = new Color(0, 0, 0, 1);
      foregroundTint.gameObject.SetActive(true);
      
      Sequence seq = DOTween.Sequence();
      seq.Append(foregroundTint.DOFade(0f, 0.5f));
      seq.AppendCallback(() => foregroundTint.gameObject.SetActive(false));
    }
  }
}