using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartsContainer;
    [SerializeField] private float heartSpacing = 40f;
    [SerializeField] private int heartsPerRow = 10;
    [SerializeField] private int healthPerHeart = 1;

    private Image[] _heartImages;

    private void Start()
    {
        
        int totalHearts = Mathf.CeilToInt(playerHealth.maxHealth / healthPerHeart);
        
        _heartImages = new Image[totalHearts];
        
        CreateHearts(totalHearts);
        
        UpdateHeartsDisplay();
        
        playerHealth.OnHealthChanged += UpdateHeartsDisplay;
    }

    private void OnDestroy()
    {
        // Отписываемся, чтобы предотвратить утечки памяти
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHeartsDisplay;
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
            GameObject newHeart = Instantiate(heartPrefab, heartsContainer);
            RectTransform heartRect = newHeart.GetComponent<RectTransform>();
            
            // Размещаем сердце в сетке
            int row = i / heartsPerRow;
            int col = i % heartsPerRow;
            heartRect.anchoredPosition = new Vector2(col * heartSpacing, -row * heartSpacing);
            
            // Сохраняем ссылку на изображение сердца для последующих обновлений
            _heartImages[i] = newHeart.GetComponent<Image>();
        }
    }

    private void UpdateHeartsDisplay()
    {
        if (playerHealth == null || _heartImages == null)
            return;
        
        float currentHealth = playerHealth.currentHealth;
        int totalHearts = _heartImages.Length;
    
        // Рассчитываем, сколько должно быть активных сердец
        int activeHearts = Mathf.CeilToInt(currentHealth / healthPerHeart);
    
        // Ограничиваем количество активных сердец максимальным количеством сердец
        activeHearts = Mathf.Min(activeHearts, totalHearts);
    
        // Обрабатываем все сердца
        for (int i = 0; i < totalHearts; i++)
        {
            if (_heartImages[i] != null)
            {
                if (i < activeHearts - 1)
                {
                    // Полное сердце
                    _heartImages[i].fillAmount = 1f;
                }
                else if (i == activeHearts - 1)
                {
                    // Последнее активное сердце может быть частично заполненным
                    float lastHeartFill = (currentHealth % healthPerHeart) / healthPerHeart;
                    if (lastHeartFill == 0) lastHeartFill = 1f; // Если кратно healthPerHeart, то полное сердце
                
                    _heartImages[i].fillAmount = lastHeartFill;
                }
                else
                {
                    // Физически удаляем лишние сердца
                    Destroy(_heartImages[i].gameObject);
                    _heartImages[i] = null;
                }
            }
        }
    }
}