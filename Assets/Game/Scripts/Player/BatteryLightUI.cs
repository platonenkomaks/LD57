using UnityEngine;
using UnityEngine.UI;

public class BatteryLightUI : MonoBehaviour
{
    [SerializeField] private BatteryLight batteryLight;
    [SerializeField] private Image batteryFillImage;

    private bool isPlayerCreated =false;

 
    public void Init()
    {
         batteryLight = G.BatteryLight;
         isPlayerCreated = true;
    }

    private void Update()
    {
        if (isPlayerCreated == false) return;

        if (batteryFillImage != null && batteryFillImage!= null ) 
        {
            batteryFillImage.fillAmount = batteryLight.GetBatteryPercentage() / 100f;
        }
        
    }
}


