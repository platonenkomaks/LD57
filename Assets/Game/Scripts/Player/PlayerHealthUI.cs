using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartsContainer;
    [SerializeField] private float heartSpacing = 40f;
    [SerializeField] private int heartsPerRow = 10;
    [SerializeField] private int healthPerHeart = 1;

    private Image[] _heartImages;
    private PlayerHealth _playerHealth;

    private void Start()
    {
        _playerHealth = G.PlayerHealth;
        if (_playerHealth == null)
        {
            Debug.LogError("PlayerHealth не найден в глобальном контексте");
            return;
        }
        
        int totalHearts = Mathf.CeilToInt(_playerHealth.maxHealth / healthPerHeart);
        
        _heartImages = new Image[totalHearts];
        
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
                GameObject newHeart = Instantiate(heartPrefab, heartsContainer);
                RectTransform heartRect = newHeart.GetComponent<RectTransform>();

                // Размещаем сердце в сетке
                int row = i / heartsPerRow;
                int col = i % heartsPerRow;
                heartRect.anchoredPosition = new Vector2(col * heartSpacing, -row * heartSpacing);
            }
        }
        else if (currentHeartCount > currentHealth)
        {
            // Удаляем лишние сердца
            for (int i = currentHeartCount - 1; i >= currentHealth; i--)
            {
                Destroy(heartsContainer.GetChild(i).gameObject);
            }
        }
    }
}