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
        G.AudioManager?.Play("LightSwitch");
        G.EventManager.Trigger(new OnPlatformEnter());
        G.Player.BatteryLight.TurnOff();
      }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
      if (collision.CompareTag("Player"))
      { 
        G.AudioManager?.Play("LightSwitch");
        G.EventManager.Trigger(new OnPlatformExit());
        G.Player.BatteryLight.TurnOn();
      }
    }
  }
}