using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Stats.BaseClasses {
  public abstract class StatSOBase : ScriptableObject {
    public string StatName;
    public string Description;
    public Sprite Image;
    public Sprite Label;
    public readonly UnityEvent OnUpgrade = new();
    
    public abstract void Initialize(int currentUpgradeIndex = -1);
    public abstract int NextUpgradeIndex { get; }
    
    public abstract void ApplyNextUpgrade();
  }
}