using System.Collections.Generic;
using Stats.BaseClasses;
using Utilities;

namespace Stats {
  public abstract class StatSO<T> : StatSOBase {
    public List<T> Upgrades;
    public Observable<T> Stat { get; private set; } = new();

    public override bool IsMaxedOut => CurrentUpgradeIndex >= Upgrades.Count;

    public T NextUpgradeValue => Upgrades[CurrentUpgradeIndex];
    protected int CurrentUpgradeIndex;

    public override void Initialize(int startUpgradeIndex = 0) {
      IsUnlocked.Value = !NeedUnlock || CurrentUpgradeIndex > 0;
      
      CurrentUpgradeIndex = startUpgradeIndex;
      Upgrade();
    }

    public override void Upgrade() {
      if (NeedUnlock && !IsUnlocked) {
        return;
      }
      
      if (CurrentUpgradeIndex >= Upgrades.Count) {
        return;
      }

      ApplyUpgrade(Upgrades[CurrentUpgradeIndex]);
      CurrentUpgradeIndex++;
    }
    
    protected abstract void ApplyUpgrade(T newValue);
  }
}