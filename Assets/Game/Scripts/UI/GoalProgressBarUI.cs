using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class GoalProgressBarUI : MonoBehaviour
  {
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text text;
    
    private IEnumerator Start()
    {
      fill.fillAmount = 0;
      yield return null;
      
      G.GoldManager.OnGoldProgressEvent.AddListener(OnChanged);
    }

    private void OnChanged(int goldMined, int goldGoal)
    {
      float newFillAmount = (float)goldMined / goldGoal;
      fill.fillAmount = newFillAmount;
      text.text = "Gold Left to Mine: " + goldMined + " / " + goldGoal;
    }
  }
}