using System.Collections.Generic;
using Stats.BaseClasses;
using Utilities;

namespace Stats {
  public abstract class StatSO<T> : StatSOBase {
    public T StartValue;
    public List<T> Upgrades;
    public Observable<T> Stat { get; private set; } = new();

    public override int NextUpgradeIndex => _nextUpgradeIndex;
    private int _nextUpgradeIndex;

    public override void Initialize(int currentUpgradeIndex = -1) {
      _nextUpgradeIndex = currentUpgradeIndex + 1;
      ApplyUpgrade(StartValue);
      
      if (currentUpgradeIndex >= 0)
      {
        ApplyNextUpgrade();
      }
    }

    public override void ApplyNextUpgrade() {
      if (NextUpgradeIndex >= Upgrades.Count) {
        return;
      }
      
      ApplyUpgrade(Upgrades[NextUpgradeIndex]);
      _nextUpgradeIndex++;
      OnUpgrade?.Invoke();
    }
    
    protected abstract void ApplyUpgrade(T newValue);
  }
}