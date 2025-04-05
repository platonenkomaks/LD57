using UnityEngine;
using Utilities;

namespace Stats.BaseClasses {
  public abstract class StatSOBase : ScriptableObject {
    public string StatName;
    public Sprite Image;
    public Sprite Label;
    public bool NeedUnlock;
    public Observable<bool> IsUnlocked { get; } = new();
    
    public abstract void Initialize(int startUpgradeIndex = 0);
    public abstract bool IsMaxedOut { get; }
    
    public abstract void Upgrade();
    
    public void Unlock() {
      this.IsUnlocked.Value = true;
    }
  }
}