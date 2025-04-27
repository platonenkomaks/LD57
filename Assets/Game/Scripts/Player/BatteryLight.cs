
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using Random = UnityEngine.Random;

public class BatteryLight : MonoBehaviour
{
    #region Настройки света и батареи

    [Header("Light Settings")]
    [SerializeField] private Light2D targetLight;

    [Header("Battery Settings")] 
    [SerializeField] private float baseBatteryLife = 60f; // базовое время работы в секундах
    [SerializeField] private float initialLightRadius = 5f;
    [SerializeField] private float minLightRadius = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float batteryChargePercentage = 1f;

    [Header("Battery Drain Settings")]
    [SerializeField] private AnimationCurve drainCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [HideInInspector] public bool isDraining = false;

    #endregion

    #region Настройки мерцания

    [Header("Battery Flicker")] 
    [SerializeField] private bool enableFlicker = true;
    [SerializeField] private float flickerThreshold = 0.3f; // начинает мерцать, когда заряд < 30%
    [SerializeField] private float flickerIntensity = 0.2f;
    [SerializeField] private float flickerSpeed = 5f;
    [SerializeField] private float severeFlickerThreshold = 0.1f; // усиленное мерцание при < 10%
    [SerializeField] private float severeFlickerIntensity = 0.5f;
    [SerializeField] private float severeFlickerSpeed = 8f;
    [SerializeField] private bool randomizeFlicker = true;

    #endregion

    #region Звуковые эффекты

    [Header("Sound Effects")]
    [SerializeField] private float lowBatterySoundThreshold = 0.25f;
    [SerializeField] private float criticalBatterySoundThreshold = 0.1f;
    [SerializeField] private float lowBatteryBeepInterval = 5f;
    [SerializeField] private float criticalBatteryBeepInterval = 2f;

    #endregion

    #region Визуальные эффекты

    [Header("Visual Effects")]
    [SerializeField] private Color normalLightColor = Color.white;
    [SerializeField] private Color lowBatteryColor = new Color(1f, 0.8f, 0.6f); // теплый желтоватый цвет
    [SerializeField] private ParticleSystem sparkParticleSystem;

    #endregion

    #region Приватные поля

    private bool canDrainBattery = false;
    private float remainingBatteryLife;
    private float originalIntensity;
    private float targetRadius;
    private float nextSoundTime;
    private bool isInCriticalMode = false;
    private System.Random rand;

    private float MaxBatteryLife => G.StatSystem.BatteryPower;

    #endregion

    #region Unity методы

    private void Awake()
    {
        targetLight = GetComponent<Light2D>();
        G.BatteryLight = this;
    }

    private void Start()
    {
        InitializeLight();
        G.EventManager.Register<OnGameStateChangedEvent>(OnGameStateChange);
    }
    
    private void OnDestroy()
    {
        G.EventManager.Unregister<OnGameStateChangedEvent>(OnGameStateChange);
    }

    private void Update()
    {
        if (!isDraining) return;
        
        UpdateBatteryLife();
        UpdateLightRadius();
        HandleLowBatteryEffects();
        CheckBatteryDepletion();
    }

    #endregion

    #region Основные методы

    private void InitializeLight()
    {
        remainingBatteryLife = MaxBatteryLife;
        originalIntensity = targetLight.intensity;
        targetLight.pointLightOuterRadius = initialLightRadius;
        rand = new System.Random();
    }

    private void UpdateBatteryLife()
    {
        remainingBatteryLife = Mathf.Max(0f, remainingBatteryLife - Time.deltaTime);
        batteryChargePercentage = remainingBatteryLife / MaxBatteryLife;
    }

    private void UpdateLightRadius()
    {
        float drainFactor = drainCurve.Evaluate(1f - batteryChargePercentage);
        targetRadius = Mathf.Lerp(initialLightRadius, minLightRadius, drainFactor);
        targetLight.pointLightOuterRadius = targetRadius;
    }

    private void CheckBatteryDepletion()
    {
        if (remainingBatteryLife <= 0f)
        {
            targetLight.intensity = 0f;
            isDraining = false;
            G.AudioManager.Stop("LightDysfunction");
            G.AudioManager.Play("LightSwitch");
            G.PlayerController.Die();
        }
    }

    #endregion

    #region Эффекты низкого заряда

    private void HandleLowBatteryEffects()
    {
        UpdateLightColor();
        HandleFlickerEffect();
        HandleSoundEffects();
    }

    private void UpdateLightColor()
    {
        if (batteryChargePercentage < lowBatterySoundThreshold)
        {
            float colorBlend = 1 - (batteryChargePercentage / lowBatterySoundThreshold);
            targetLight.color = Color.Lerp(normalLightColor, lowBatteryColor, colorBlend);
        }
        else
        {
            targetLight.color = normalLightColor;
        }
    }

    private void HandleFlickerEffect()
    {
        if (!enableFlicker)
        {
            targetLight.intensity = originalIntensity;
            return;
        }

        float flickerValue = CalculateFlickerValue();
        targetLight.intensity = originalIntensity * (1f - flickerValue);
    }

    private float CalculateFlickerValue()
    {
        if (batteryChargePercentage < severeFlickerThreshold)
        {
            return CalculateSevereFlicker();
        }
        else if (batteryChargePercentage < flickerThreshold)
        {
            return CalculateNormalFlicker();
        }
        return 0f;
    }

    private float CalculateSevereFlicker()
    {
        float flickerValue;
        if (randomizeFlicker)
        {
            flickerValue = (float)rand.NextDouble() * severeFlickerIntensity;
        }
        else
        {
            flickerValue = Mathf.Abs(Mathf.Sin(Time.time * severeFlickerSpeed)) * severeFlickerIntensity;
            if (Random.value < 0.05f)
            {
                flickerValue = severeFlickerIntensity;
            }
        }

        if (sparkParticleSystem != null && Random.value < 0.01f)
        {
            sparkParticleSystem.Emit(1);
        }

        return flickerValue;
    }

    private float CalculateNormalFlicker()
    {
        float intensityFactor = 1f - batteryChargePercentage / flickerThreshold;
        if (randomizeFlicker)
        {
            return (float)rand.NextDouble() * flickerIntensity * intensityFactor;
        }
        return Mathf.Sin(Time.time * flickerSpeed) * flickerIntensity * intensityFactor;
    }

    private void HandleSoundEffects()
    {
        if (Time.time <= nextSoundTime) return;

        if (batteryChargePercentage < criticalBatterySoundThreshold)
        {
            HandleCriticalBatterySound();
        }
        else if (batteryChargePercentage < lowBatterySoundThreshold)
        {
            HandleLowBatterySound();
        }
    }

    private void HandleCriticalBatterySound()
    {
        G.AudioManager.Play("LightDysfunction");
        nextSoundTime = Time.time + criticalBatteryBeepInterval;

        if (!isInCriticalMode)
        {
            isInCriticalMode = true;
            StartCoroutine(PulsateLight());
        }
    }

    private void HandleLowBatterySound()
    {
        isInCriticalMode = false;
        G.AudioManager.Play("lowBatteryBeep");
        nextSoundTime = Time.time + lowBatteryBeepInterval;
    }

    #endregion

    #region Публичные методы

    public void RechargeBattery(float chargeAmount)
    {
        remainingBatteryLife += chargeAmount;
        remainingBatteryLife = Mathf.Clamp(remainingBatteryLife, 0f, MaxBatteryLife);
        batteryChargePercentage = remainingBatteryLife / MaxBatteryLife;

        if (!targetLight.enabled && batteryChargePercentage > 0)
        {
            isInCriticalMode = false;
        }
    }

    public void FullRecharge()
    {
        batteryChargePercentage = 1f;
        remainingBatteryLife = MaxBatteryLife;
        isInCriticalMode = false;
    }

    public void TurnOn()
    {
        if (batteryChargePercentage <= 0) return;
        
        if (canDrainBattery)
            isDraining = true;
        
        targetLight.enabled = true;
    }
    
    public void TurnOff()
    {
        isDraining = false;
        targetLight.enabled = false;
    }

    public int GetBatteryPercentage()
    {
        return Mathf.RoundToInt(batteryChargePercentage * 100f);
    }
    
    public float GetMaxBatteryLifeSeconds()
    {
        return remainingBatteryLife;
    }

    #endregion

    #region Корутины

    private IEnumerator PulsateLight()
    {
        float originalRadius = targetLight.pointLightOuterRadius;
        float pulseFactor = 1.2f;

        while (isInCriticalMode && batteryChargePercentage > 0)
        {
            yield return StartCoroutine(PulseCycle(originalRadius, pulseFactor));
            yield return new WaitForSeconds(0.5f);
            originalRadius = targetRadius;
        }
    }

    private IEnumerator PulseCycle(float originalRadius, float pulseFactor)
    {
        float duration = 0.2f;
        
        // Увеличение радиуса
        yield return StartCoroutine(LerpLightRadius(originalRadius, originalRadius * pulseFactor, duration));
        
        // Уменьшение радиуса
        yield return StartCoroutine(LerpLightRadius(originalRadius * pulseFactor, originalRadius, duration));
    }

    private IEnumerator LerpLightRadius(float startRadius, float endRadius, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float progress = (Time.time - startTime) / duration;
            targetLight.pointLightOuterRadius = Mathf.Lerp(startRadius, endRadius, progress);
            yield return null;
        }
    }

    #endregion

    private void OnGameStateChange(OnGameStateChangedEvent e)
    {
        switch (e.State)
        {
            case GameLoopStateMachine.GameLoopState.Tutorial:
            case GameLoopStateMachine.GameLoopState.Shopping:
                canDrainBattery = false;
                break;
            case GameLoopStateMachine.GameLoopState.Mining:
            case GameLoopStateMachine.GameLoopState.Descend:
            case GameLoopStateMachine.GameLoopState.Ascend:
            case GameLoopStateMachine.GameLoopState.Win:
            default:
                canDrainBattery = true;
                break;
        }
    }

    #region Отладка

    private void OnDrawGizmos()
    {
        if (targetLight != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, targetLight.pointLightOuterRadius);
        }
    }

    #endregion
}