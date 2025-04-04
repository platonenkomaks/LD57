using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string startScene = "MainMenu";
    [SerializeField] private GameObject[] requiredSystems;
    
    private void Awake()
    {
        // Инициализация всех необходимых систем, которые должны существовать
        // на протяжении всей игры (Сингл тоны)
        InitializeRequiredSystems();
        
        // Настройка приложения
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        
        // Предотвращаем уничтожение этого объекта при загрузке новой сцены
        DontDestroyOnLoad(gameObject);
        
        // Переход на стартовую сцену
        if (SceneManager.GetActiveScene().name != startScene)
        {
            SceneManager.LoadScene(startScene);
        }
    }
    
    private void InitializeRequiredSystems()
    {
        // Создаем все необходимые системы (если их еще нет)
        foreach (var systemPrefab in requiredSystems)
        {
            var systemName = systemPrefab.name;
            
            // Проверяем, существует ли уже система
            if (GameObject.Find(systemName) != null) continue;
            // Создаем новый экземпляр системы
            var system = Instantiate(systemPrefab);
            system.name = systemName;
                
            // Предотвращаем уничтожение системы при переходе между сценами
            DontDestroyOnLoad(system);
        }
    }
}

