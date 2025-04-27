using System;
using Events;
using UnityEngine;

namespace Platform
{
  public class PlatformArea : MonoBehaviour
  {
    
    public bool IsInArea { get; private set; } = false;

    public void Awake()
    {
      G.PlatformArea = this;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
      if (collision.CompareTag("Player"))
      {
        IsInArea = true;
        G.AudioManager?.Play("LightSwitch");
        G.EventManager.Trigger(new OnPlatformEnter());
        G.Player.BatteryLight.TurnOff();
      }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
      if (collision.CompareTag("Player"))
      { 
        IsInArea = false;
        G.AudioManager?.Play("LightSwitch");
        G.EventManager.Trigger(new OnPlatformExit());
        G.Player.BatteryLight.TurnOn();
      }
    }
  }
}