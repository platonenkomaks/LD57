using System.Collections;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;

namespace Platform
{
  public class StateCollider : MonoBehaviour
  {
    [SerializeField] private GameLoopStateMachine.GameLoopState disableState;
    private BoxCollider2D _collider;

    private void Awake()
    {
      _collider = GetComponent<BoxCollider2D>();
    }

    private IEnumerator Start()
    {
      yield return null;
      G.EventManager.Register<OnGameStateChangedEvent>(OnStateChange);
    }
    
    private void OnDestroy()
    {
      G.EventManager.Unregister<OnGameStateChangedEvent>(OnStateChange);
    }
    
    private void OnStateChange(OnGameStateChangedEvent e)
    {
      _collider.enabled = e.State != disableState;
    }
  }
}