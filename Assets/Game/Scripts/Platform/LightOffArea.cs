using UnityEngine;

namespace Platform
{
  public class LightOffArea : MonoBehaviour
  {
    private void OnTriggerEnter2D(Collider2D collision)
    {
      if (collision.CompareTag("Player"))
      {
        G.Player.BatteryLight.isDraining = false;
      }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
      if (collision.CompareTag("Player"))
      {
        G.Player.BatteryLight.isDraining = true;
      }
    }
  }
}