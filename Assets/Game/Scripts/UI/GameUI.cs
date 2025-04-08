using DG.Tweening;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class GameUI : MonoBehaviour
  {
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Image foregroundTint;
    
    private void Start()
    {
      G.EventManager.Register<OnPlayerDeath>(OnGameOver);
      DoFadeOut();
    }

    private void OnDestroy()
    {
      G.EventManager.Unregister<OnPlayerDeath>(OnGameOver);
    }

    private void OnGameOver(OnPlayerDeath e)
    {
      _gameOverPanel.SetActive(true);
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