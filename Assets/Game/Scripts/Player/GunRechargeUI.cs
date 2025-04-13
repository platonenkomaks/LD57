using UnityEngine;
using UnityEngine.UI;
using Events;

public class GunRechargeUI : MonoBehaviour
{
    [SerializeField] private Image rechargeImage;
    
    private float _currentRechargeTime = 0f;
    private bool _isRecharging = false;
    private bool _isCombatMode = false;
    
    private void Start()
    {
        if (rechargeImage == null)
        {
            Debug.LogError("RechargeImage не назначен в GunRechargeUI");
            return;
        }
        
        // Подписываемся на событие выстрела
        G.EventManager.Register<OnPlayerShoot>(OnPlayerShootEvent);
        
        // Подписываемся на событие изменения состояния игрока
        G.EventManager.Register<OnPlayerStateChangeEvent>(OnPlayerStateChange);
        
        // Изначально скрываем UI
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        // Отписываемся от событий
        G.EventManager.Unregister<OnPlayerShoot>(OnPlayerShootEvent);
        G.EventManager.Unregister<OnPlayerStateChangeEvent>(OnPlayerStateChange);
    }
    
    private void Update()
    {
        if (_isRecharging)
        {
            _currentRechargeTime += Time.deltaTime;
            float cooldown = G.StatSystem.ShootgunCooldown;
            float fillAmount = _currentRechargeTime / cooldown;
            rechargeImage.fillAmount = fillAmount;
            
            if (_currentRechargeTime >= cooldown)
            {
                _isRecharging = false;
                rechargeImage.fillAmount = 1f;
            }
        }
    }
    
    private void OnPlayerShootEvent(OnPlayerShoot evt)
    {
        // При выстреле сбрасываем заливку и начинаем перезарядку
        rechargeImage.fillAmount = 0f;
        _currentRechargeTime = 0f;
        _isRecharging = true;
    }
    
    private void OnPlayerStateChange(OnPlayerStateChangeEvent evt)
    {
        // Показываем UI только в состоянии драки
        bool shouldShow = evt.State == PlayerStateMachine.PlayerState.Fighting;
        
        // Если состояние изменилось, обновляем видимость
        if (_isCombatMode != shouldShow)
        {
            _isCombatMode = shouldShow;
            gameObject.SetActive(_isCombatMode);
        }
    }
}



