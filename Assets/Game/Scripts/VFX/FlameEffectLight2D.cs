using UnityEngine;
 using UnityEngine.Rendering.Universal; // Для Unity 2021+

public class FlameEffectLight2D : MonoBehaviour
{
    [Header("Интенсивность")]
    [SerializeField] private float baseIntensity = 1.0f;
    [SerializeField] private float intensityVariation = 0.2f;
    
    [Header("Радиус")]
    [SerializeField] private float baseRadius = 3.0f;
    [SerializeField] private float radiusVariation = 0.5f;
    
    [Header("Цвет")]
    [SerializeField] private Color baseColor = new Color(1.0f, 0.7f, 0.3f); // Основной цвет огня
    [SerializeField] private Color flickerColor = new Color(1.0f, 0.5f, 0.2f); // Цвет мерцания
    [SerializeField] private float colorVariationIntensity = 0.2f; // Сила изменения цвета
    
    [Header("Скорость изменений")]
    [SerializeField] private float flickerSpeed = 5.0f; // Скорость мерцания
    [SerializeField] private Vector2 noiseScale = new Vector2(1.0f, 1.0f); // Масштаб шума
    
    [Header("Дополнительные эффекты")]
    [SerializeField] private bool useWindEffect = true; // Использовать эффект ветра
    [SerializeField] private float windStrength = 0.1f; // Сила ветра
    [SerializeField] private float windFrequency = 0.2f; // Частота ветра
    
    private Light2D light2D;
    private float initialX;
    private float timeOffset;
    
    private void Start()
    {
        light2D = GetComponent<Light2D>();
        if (light2D == null)
        {
            Debug.LogError("Light2D компонент не найден!");
            enabled = false;
            return;
        }
        
        // Сохранение исходной позиции
        initialX = transform.position.x;
        
        // Случайное смещение времени для разных источников огня
        timeOffset = Random.Range(0f, 100f);
    }
    
    private void Update()
    {
        float time = Time.time + timeOffset;
        
        // Создание эффекта огня с помощью шума Перлина для плавных изменений
        float noise1 = Mathf.PerlinNoise(time * flickerSpeed * noiseScale.x, 0);
        float noise2 = Mathf.PerlinNoise(0, time * flickerSpeed * noiseScale.y);
        float combinedNoise = (noise1 + noise2) * 0.5f;
        
        // Изменение интенсивности света
        light2D.intensity = baseIntensity + (combinedNoise * 2 - 1) * intensityVariation;
        
        // Изменение радиуса внешнего круга света
        if (light2D.pointLightOuterRadius > 0)
        {
            light2D.pointLightOuterRadius = baseRadius + (combinedNoise * 2 - 1) * radiusVariation;
            // Внутренний радиус также можно менять
            light2D.pointLightInnerRadius = light2D.pointLightOuterRadius * 0.4f;
        }
        
        // Плавное изменение цвета
        light2D.color = Color.Lerp(baseColor, flickerColor, combinedNoise * colorVariationIntensity);
        
        // Эффект колебания от ветра (смещение позиции источника света)
        if (useWindEffect)
        {
            float windEffect = Mathf.Sin(time * windFrequency) * windStrength;
            Vector3 position = transform.position;
            position.x = initialX + windEffect;
            transform.position = position;
        }
    }
}