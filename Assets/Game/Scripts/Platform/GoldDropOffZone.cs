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
      
      if (G.BackPack.IsEmpty()) return;
      
      G.ElevatorPlatform.GetComponent<PlatformWeight>().AddGold(G.BackPack.CurrentGold);
      G.BackPack.ResetGold();
      G.AudioManager.Play("DropGold");
      G.MiningSystem.EnableMining();
      
    }
  }
}