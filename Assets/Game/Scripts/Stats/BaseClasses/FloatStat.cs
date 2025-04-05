using UnityEngine;

namespace Stats.BaseClasses {
  [CreateAssetMenu(fileName = "FloatStat", menuName = "Stats/Float Stat")]
  public class FloatStat : StatSO<float> {
    
    protected override void ApplyUpgrade(float newValue) {
      Stat.Value = newValue;
    }
  }
}