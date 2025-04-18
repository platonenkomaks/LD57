using System;
using Stats.BaseClasses;
using UnityEngine;

namespace GameControl
{
  public class StatSystem : MonoBehaviour
  {
    [SerializeField] private FloatStat elevatorSpeed;
    [SerializeField] private FloatStat batteryPower;
    [SerializeField] private FloatStat shotgunCooldown;

    public float ShootgunCooldown => shotgunCooldown.Stat.Value;
    
    private void Start()
    {
      G.StatSystem = this;
      elevatorSpeed.Initialize();
      batteryPower.Initialize();
      shotgunCooldown.Initialize();
    }

    private void OnDestroy()
    {
      G.StatSystem = null;
    }
  }
}