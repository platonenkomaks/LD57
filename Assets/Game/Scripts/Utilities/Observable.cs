using System;
using UnityEngine;

namespace Utilities
{
  [Serializable]
  public class Observable<T> {
    [SerializeField] private T value;

    public Observable() {}

    public Observable(T value) {
      this.value = value;
    }

    /// <summary>
    /// Triggers with arguments: this, oldValue, newValue
    /// </summary>
    public Action<Observable<T>, T, T> OnChanged;

    public T Value {
      get => value;
      set {
        T oldValue = this.value;
        this.value = value;
        OnChanged?.Invoke(this, oldValue, value);
      }
    }

    public static implicit operator T(Observable<T> observable) => observable.value;

    public override string ToString() {
      return value.ToString();
    }
  }
}