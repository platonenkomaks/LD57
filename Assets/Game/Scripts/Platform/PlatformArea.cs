using System;
using Events;
using UnityEngine;

namespace Platform
{
  public class PlatformArea : MonoBehaviour
  {
    private bool isWorking = true;
    
    private void Start()
    {
      G.EventManager.Register<OnPlayerDeath>(OnGameOver);
    }

    private void OnDestroy()
    {
      G.EventManager.Unregister<OnPlayerDeath>(OnGameOver);
    }
    
    private void OnGameOver(OnPlayerDeath e)
    {
      this.gameObject.SetActive(false);
    }

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