using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartsContainer;
    [SerializeField] private int healthPerHeart = 1;
    
    private PlayerHealth _playerHealth;

    private void Start()
    {
        _playerHealth = G.PlayerHealth;
        if (_playerHealth == null)
        {
            Debug.LogError("PlayerHealth не найден в глобальном контексте");
            return;
        }
        
        int totalHearts = Mathf.CeilToInt((float)_playerHealth.maxHealth / healthPerHeart);
        
        CreateHearts(totalHearts);
        
        UpdateHeartsDisplay();
        
        _playerHealth.OnHealthChanged += UpdateHeartsDisplay;
    }
    

    private void OnDestroy()
    {
        // Отписываемся, чтобы предотвратить утечки памяти
        if (_playerHealth != null)
        {
            _playerHealth.OnHealthChanged -= UpdateHeartsDisplay;
        }
    }

    private void CreateHearts(int count)
    {
        // Сначала очищаем все существующие сердца
        foreach (Transform child in heartsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Создаем все сердца
        for (int i = 0; i < count; i++)
        {
            Instantiate(heartPrefab, heartsContainer);
        }
    }

    public void UpdateHeartsDisplay()
    {
        if (_playerHealth == null)
            return;

        int currentHealth = _playerHealth.currentHealth;

        // Убедимся, что количество сердец соответствует текущему здоровью
        int currentHeartCount = heartsContainer.childCount;
        
        if (currentHeartCount < currentHealth)
        {
            // Добавляем недостающие сердца
            for (int i = currentHeartCount; i < currentHealth; i++)
            {
                Instantiate(heartPrefab, heartsContainer);
            }
        }
        else if (currentHeartCount > currentHealth && currentHeartCount > 0)
        {
            // Удаляем лишние сердца
            for (int i = currentHeartCount - 1; i >= currentHealth; i--)
            {
                Destroy(heartsContainer.GetChild(i).gameObject);
            }
        }
    }
}