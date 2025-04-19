using Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;

namespace Platform
{
  public class PlatformArea : MonoBehaviour
  {
    private bool _isWorking = true;
    
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
      _isWorking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      if (collision.CompareTag("Player") && _isWorking)
      {
        G.AudioManager.Play("LightSwitch");
        G.EventManager.Trigger(new OnPlatformEnter());
        G.Player.BatteryLight.TurnOff();
      }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
      if (collision.CompareTag("Player") && _isWorking)
      { 
        G.AudioManager.Play("LightSwitch");
        G.EventManager.Trigger(new OnPlatformExit());
        G.Player.BatteryLight.TurnOn();
      }
    }
  }
}