using UnityEngine;
using UnityEngine.UI;
using Events;

public class BatteryLightUI : MonoBehaviour
{
    [SerializeField] private BatteryLight batteryLight;
    [SerializeField] private Image batteryFillImage;

    private bool isPlayerCreated = false;
    private bool isVisible = false;

    public void Init()
    {
        batteryLight = G.BatteryLight;
        isPlayerCreated = true;
        
        // Подписываемся на событие изменения состояния игрока
        G.EventManager.Register<OnPlayerStateChangeEvent>(OnPlayerStateChange);
        
        // Изначально скрываем UI
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        // Отписываемся от события изменения состояния игрока
        G.EventManager.Unregister<OnPlayerStateChangeEvent>(OnPlayerStateChange);
    }

    private void Update()
    {
        if (isPlayerCreated == false) return;

        if (batteryFillImage != null && batteryLight != null) 
        {
            batteryFillImage.fillAmount = batteryLight.GetBatteryPercentage() / 100f;
        }
    }
    
    private void OnPlayerStateChange(OnPlayerStateChangeEvent evt)
    {
        // Показываем UI только в состояниях добычи золота и транспортировки
        bool shouldShow = evt.State == PlayerStateMachine.PlayerState.Mining || 
                         evt.State == PlayerStateMachine.PlayerState.Carrying;
        
        // Если состояние изменилось, обновляем видимость
        if (isVisible != shouldShow)
        {
            isVisible = shouldShow;
            gameObject.SetActive(isVisible);
        }
    }
}


