using System;
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
    [SerializeField] private float maxBatteryLife = 60f; // время работы в секундах
    [SerializeField] private float initialLightRadius = 5f;
    [SerializeField] private float minLightRadius = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float currentBatteryCharge = 1f;

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

    private bool _isTutorialComplete = false;
    private float remainingBatteryLife;
    private float originalIntensity;
    private float targetRadius;
    private float nextSoundTime;
    private bool isInCriticalMode = false;
    private System.Random rand;

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
        remainingBatteryLife = maxBatteryLife * currentBatteryCharge;
        originalIntensity = targetLight.intensity;
        targetLight.pointLightOuterRadius = initialLightRadius;
        rand = new System.Random();
    }

    private void UpdateBatteryLife()
    {
        remainingBatteryLife = Mathf.Max(0f, remainingBatteryLife - Time.deltaTime);
        currentBatteryCharge = remainingBatteryLife / maxBatteryLife;
    }

    private void UpdateLightRadius()
    {
        float drainFactor = drainCurve.Evaluate(1f - currentBatteryCharge);
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
        if (currentBatteryCharge < lowBatterySoundThreshold)
        {
            float colorBlend = 1 - (currentBatteryCharge / lowBatterySoundThreshold);
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
        if (currentBatteryCharge < severeFlickerThreshold)
        {
            return CalculateSevereFlicker();
        }
        else if (currentBatteryCharge < flickerThreshold)
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
        float intensityFactor = 1f - currentBatteryCharge / flickerThreshold;
        if (randomizeFlicker)
        {
            return (float)rand.NextDouble() * flickerIntensity * intensityFactor;
        }
        return Mathf.Sin(Time.time * flickerSpeed) * flickerIntensity * intensityFactor;
    }

    private void HandleSoundEffects()
    {
        if (Time.time <= nextSoundTime) return;

        if (currentBatteryCharge < criticalBatterySoundThreshold)
        {
            HandleCriticalBatterySound();
        }
        else if (currentBatteryCharge < lowBatterySoundThreshold)
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
        currentBatteryCharge = Mathf.Clamp01(currentBatteryCharge + chargeAmount);
        remainingBatteryLife = maxBatteryLife * currentBatteryCharge;

        if (!targetLight.enabled && currentBatteryCharge > 0)
        {
            isInCriticalMode = false;
        }
    }

    public void FullRecharge()
    {
        RechargeBattery(1f);
    }

    public void TurnOn()
    {
        if (currentBatteryCharge <= 0) return;
        
        if (_isTutorialComplete)
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
        return Mathf.RoundToInt(currentBatteryCharge * 100f);
    }

    #endregion

    #region Корутины

    private IEnumerator PulsateLight()
    {
        float originalRadius = targetLight.pointLightOuterRadius;
        float pulseFactor = 1.2f;

        while (isInCriticalMode && currentBatteryCharge > 0)
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
        if (e.State != GameLoopStateMachine.GameLoopState.Tutorial)
        {
            _isTutorialComplete = true;
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