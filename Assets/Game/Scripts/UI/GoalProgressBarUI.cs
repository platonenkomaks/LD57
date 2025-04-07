using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
  public class GoalProgressBarUI : MonoBehaviour
  {
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text text;

    private int _goalProgress = 0;
    
    private IEnumerator Start()
    {
      fill.fillAmount = 0;
      yield return null;
      G.GoldManager.goldGoalProgress01.OnChanged += OnChanged;
    }

    private void OnChanged(Observable<float> _, float oldVal, float newVal)
    {
      _goalProgress++;
      fill.fillAmount = newVal;
      text.text = _goalProgress + " / " + G.GoldManager.GoldGoal;
    }
  }
}