using UnityEngine;

namespace Stats.BaseClasses {
  [CreateAssetMenu(fileName = "IntStat", menuName = "Stats/Int Stat")]
  public class IntStat : StatSO<int> {
    protected override void ApplyUpgrade(int newValue) {
      Stat.Value = newValue;
      OnUpgrade?.Invoke();
    }
  }
}