using Events;
using UnityEngine;

namespace Platform
{
  public class PlatformArea : MonoBehaviour
  {
    private void OnTriggerEnter2D(Collider2D collision)
    {
      if (collision.CompareTag("Player"))
      {
        G.AudioManager.Play("LightSwitch");
        G.Player.BatteryLight.isDraining = false;
        G.Player.BatteryLight.TurnOff();
        G.EventManager.Trigger(new OnPlatformEnter());
      }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
      if (collision.CompareTag("Player"))
      {
        G.AudioManager.Play("LightSwitch");
        G.Player.BatteryLight.isDraining = true;
        G.Player.BatteryLight.TurnOn();
        G.EventManager.Trigger(new OnPlatformExit());
      }
    }
  }
}