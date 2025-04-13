using UnityEngine;

namespace Platform
{
  [RequireComponent(typeof(Collider2D))]
  public class GoldDropOffZone : MonoBehaviour
  {
    private void OnTriggerEnter2D(Collider2D other)
    {
      if (!other.CompareTag("Player"))
        return;
      
      if (G.PlayerStateMachine.CurrentState != PlayerStateMachine.PlayerState.Carrying)
        return;
      
      G.GoldManager.AddGold(1);
      G.AudioManager.Play("DropGold");
      G.MiningSystem.EnableMining();
    }
  }
}