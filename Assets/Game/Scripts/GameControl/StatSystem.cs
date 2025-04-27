using System;
using Stats.BaseClasses;
using UnityEngine;

namespace GameControl
{
  public class StatSystem : MonoBehaviour
  {
    [field:SerializeField] public FloatStat ElevatorSpeedStat { get; private set; }
    [field:SerializeField] public FloatStat BatteryPowerStat { get; private set; }
    [field:SerializeField] public FloatStat ShotgunCooldownStat { get; private set; }

    public float ShotgunCooldown => ShotgunCooldownStat.Stat.Value;
    public float BatteryPower => BatteryPowerStat.Stat.Value;
    public float ElevatorSpeed => ElevatorSpeedStat.Stat.Value;
    
    private void Awake()
    {
      G.StatSystem = this;
      ElevatorSpeedStat.Initialize();
      BatteryPowerStat.Initialize();
      ShotgunCooldownStat.Initialize();
    }

    private void OnDestroy()
    {
      G.StatSystem = null;
    }
  }
}