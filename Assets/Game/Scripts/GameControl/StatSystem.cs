using Stats.BaseClasses;
using UnityEngine;

namespace GameControl
{
  public class StatSystem : MonoBehaviour
  {
    [SerializeField] private FloatStat elevatorSpeed;
    [SerializeField] private FloatStat batteryPower;
    [SerializeField] private FloatStat shotgunCooldown;

    private void Start()
    {
      G.StatSystem = this;
      elevatorSpeed.Initialize();
      batteryPower.Initialize();
      shotgunCooldown.Initialize();
    }
  }
}