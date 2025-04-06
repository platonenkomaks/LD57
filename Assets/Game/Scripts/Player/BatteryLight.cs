using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BatteryLight : MonoBehaviour
{
    [Header("Light Settings")] [SerializeField]
    private Light2D targetLight;

    [Header("Battery Settings")] [SerializeField]
    private float maxBatteryLife = 60f; // время работы в секундах

    [SerializeField] private float initialLightRadius = 5f;
    [SerializeField] private float minLightRadius = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float currentBatteryCharge = 1f;

    [Header("Battery Drain Settings")] [SerializeField]
    private AnimationCurve drainCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [HideInInspector] public bool isDraining = false;

    [Header("Battery Flicker")] [SerializeField]
    private bool enableFlicker = true;

    [SerializeField] private float flickerThreshold = 0.3f; // начинает мерцать, когда заряд < 30%
    [SerializeField] private float flickerIntensity = 0.2f;
    [SerializeField] private float flickerSpeed = 5f;
    [SerializeField] private float severeFlickerThreshold = 0.1f; // усиленное мерцание при < 10%
    [SerializeField] private float severeFlickerIntensity = 0.5f;
    [SerializeField] private float severeFlickerSpeed = 8f;
    [SerializeField] private bool randomizeFlicker = true;

    [Header("Sound Effects")] [SerializeField]
    private float lowBatterySoundThreshold = 0.25f;

    [SerializeField] private float criticalBatterySoundThreshold = 0.1f;
    [SerializeField] private float lowBatteryBeepInterval = 5f;
    [SerializeField] private float criticalBatteryBeepInterval = 2f;

    [Header("Visual Effects")] [SerializeField]
    private Color normalLightColor = Color.white;

    [SerializeField] private Color lowBatteryColor = new Color(1f, 0.8f, 0.6f); // теплый желтоватый цвет
    [SerializeField] private ParticleSystem sparkParticleSystem;

    private float remainingBatteryLife;
    private float originalIntensity;
    private float targetRadius;
    private float nextSoundTime;
    private bool isInCriticalMode = false;
    private System.Random rand;

    private void Start()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light2D>();

            if (targetLight == null)
            {
                Debug.LogError("Light2D component not found!");
                enabled = false;
                return;
            }
        }

        // Инициализируем начальные настройки
        remainingBatteryLife = maxBatteryLife * currentBatteryCharge;
        originalIntensity = targetLight.intensity;
        targetLight.pointLightOuterRadius = initialLightRadius;
        rand = new System.Random();
    }

    private void Update()
    {
        if (!isDraining)
            return;
        
        // Уменьшаем оставшееся время работы батареи
        remainingBatteryLife -= Time.deltaTime;
        remainingBatteryLife = Mathf.Max(0f, remainingBatteryLife);

        // Обновляем текущий заряд батареи (от 0 до 1)
        currentBatteryCharge = remainingBatteryLife / maxBatteryLife;

        // Рассчитываем новый радиус света на основе кривой расхода
        float drainFactor = drainCurve.Evaluate(1f - currentBatteryCharge);
        targetRadius = Mathf.Lerp(initialLightRadius, minLightRadius, drainFactor);

        // Применяем новый радиус к свету
        targetLight.pointLightOuterRadius = targetRadius;

        // Работа с эффектами при низком заряде
        HandleLowBatteryEffects();

        // Выключаем свет, если батарея полностью разряжена
        if (remainingBatteryLife <= 0f)
        {
            targetLight.intensity = 0f;
            isDraining = false;
            G.AudioManager.Stop("LightDysfunction");
            G.AudioManager.Play("LightSwitch");
        }
    }

    private void HandleLowBatteryEffects()
    {
        // Изменение цвета при низком заряде
        if (currentBatteryCharge < lowBatterySoundThreshold)
        {
            // Плавно меняем цвет от нормального к "разряженному"
            float colorBlend = 1 - (currentBatteryCharge / lowBatterySoundThreshold);
            targetLight.color = Color.Lerp(normalLightColor, lowBatteryColor, colorBlend);
        }
        else
        {
            targetLight.color = normalLightColor;
        }

        // Обработка мерцания
        if (enableFlicker)
        {
            float flickerValue = 0;

            if (currentBatteryCharge < severeFlickerThreshold)
            {
                // Очень сильное мерцание при критическом заряде
                if (randomizeFlicker)
                {
                    // Случайное мерцание
                    flickerValue = (float)rand.NextDouble() * severeFlickerIntensity;
                }
                else
                {
                    // Синусоидальное мерцание с возможностью "выпадений"
                    flickerValue = Mathf.Abs(Mathf.Sin(Time.time * severeFlickerSpeed)) * severeFlickerIntensity;
                    if (Random.value < 0.05f) // 5% шанс "выпадения" света
                    {
                        flickerValue = severeFlickerIntensity;
                    }
                }

                // Запускаем искры при критическом заряде
                if (sparkParticleSystem != null && Random.value < 0.01f)
                {
                    sparkParticleSystem.Emit(1);
                }
            }
            else if (currentBatteryCharge < flickerThreshold)
            {
                // Стандартное мерцание при низком заряде
                if (randomizeFlicker)
                {
                    flickerValue = (float)rand.NextDouble() * flickerIntensity *
                                   (1f - currentBatteryCharge / flickerThreshold);
                }
                else
                {
                    flickerValue = Mathf.Sin(Time.time * flickerSpeed) * flickerIntensity *
                                   (1f - currentBatteryCharge / flickerThreshold);
                }
            }

            targetLight.intensity = originalIntensity * (1f - flickerValue);
        }
        else
        {
            targetLight.intensity = originalIntensity;
        }

        // Звуковые эффекты
        if (Time.time > nextSoundTime)
        {
            if (currentBatteryCharge < criticalBatterySoundThreshold)
            {
                G.AudioManager.Play("LightDysfunction");
                nextSoundTime = Time.time + criticalBatteryBeepInterval;

                // Если только что вошли в критический режим
                if (!isInCriticalMode)
                {
                    isInCriticalMode = true;
                    StartCoroutine(PulsateLight());
                }
            }
            else if (currentBatteryCharge < lowBatterySoundThreshold)
            {
                // Низкий уровень заряда
                isInCriticalMode = false;

                G.AudioManager.Play("lowBatteryBeep");
                nextSoundTime = Time.time + lowBatteryBeepInterval;
            }
        }
    }

    // Корутина для пульсации света при критическом заряде
    private IEnumerator PulsateLight()
    {
        float originalRadius = targetLight.pointLightOuterRadius;
        float pulseFactor = 1.2f;

        while (isInCriticalMode && currentBatteryCharge > 0)
        {
            // Увеличиваем радиус
            float startTime = Time.time;
            float duration = 0.2f;

            while (Time.time - startTime < duration)
            {
                float progress = (Time.time - startTime) / duration;
                targetLight.pointLightOuterRadius = originalRadius * Mathf.Lerp(1f, pulseFactor, progress);
                yield return null;
            }

            // Уменьшаем радиус
            startTime = Time.time;
            while (Time.time - startTime < duration)
            {
                float progress = (Time.time - startTime) / duration;
                targetLight.pointLightOuterRadius = originalRadius * Mathf.Lerp(pulseFactor, 1f, progress);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            // Обновляем оригинальный радиус, так как он может измениться из-за разряда
            originalRadius = targetRadius;
        }
    }

    // Метод для подбора батареек
    public void RechargeBattery(float chargeAmount)
    {
        currentBatteryCharge = Mathf.Clamp01(currentBatteryCharge + chargeAmount);
        remainingBatteryLife = maxBatteryLife * currentBatteryCharge;

        if (!targetLight.enabled && currentBatteryCharge > 0)
        {
            targetLight.intensity = originalIntensity;
            targetLight.enabled = true;
            isInCriticalMode = false;
        }
    }

    // Метод для полной зарядки батареи
    public void FullRecharge()
    {
        RechargeBattery(1f);
    }

    // Метод для включения/выключения фонарика
    public void ToggleLight()
    {
        if (currentBatteryCharge <= 0)
            return;

        isDraining = !isDraining;
        targetLight.enabled = isDraining;
    }

    // Получить текущий процент заряда (0-100)
    public int GetBatteryPercentage()
    {
        return Mathf.RoundToInt(currentBatteryCharge * 100f);
    }

    // Для отладки в редакторе
    private void OnDrawGizmos()
    {
        if (targetLight != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, targetLight.pointLightOuterRadius);
        }
    }
}