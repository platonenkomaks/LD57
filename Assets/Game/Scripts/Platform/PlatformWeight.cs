using System;
using Events;
using UnityEngine;
using TMPro;

public class PlatformWeight : MonoBehaviour
{
    [SerializeField] private GameObject weightArrow;
    [SerializeField] private TextMeshPro weightText;
    
    public int goldOnPlatformBalance;
    
    private ElevatorPlatform _elevatorPlatform;
    private const float BaseWeight = 1f; // Default platform weight
    
    private bool isAscending = false;
    private float remainingTime = 0f;
    private float totalAscentTime = 0f;
    
    public Action OnWeightChange;
    
    private void Start()
    {
        G.EventManager.Register<OnPlayerRespawn>(OnRespawn);
        goldOnPlatformBalance = 0;
        _elevatorPlatform = G.ElevatorPlatform;
        
        // Subscribe to elevator events to track when ascent starts/ends
        if (_elevatorPlatform != null)
        {
            _elevatorPlatform.OnStartAscent += HandleAscentStart;
            _elevatorPlatform.OnArriveToSurfaceEvent += HandleAscentEnd;
        }
        
        UpdatePlatformWeight();
    }
    
    private void OnDestroy()
    {
        G.EventManager.Unregister<OnPlayerRespawn>(OnRespawn);
        
        // Unsubscribe from events when destroyed
        if (_elevatorPlatform != null)
        {
            _elevatorPlatform.OnStartAscent -= HandleAscentStart;
            _elevatorPlatform.OnArriveToSurfaceEvent -= HandleAscentEnd;
        }
    }
    
    private void OnRespawn(OnPlayerRespawn _)
    {
        ResetWeight();
    }

    private void HandleAscentStart()
    {
        isAscending = true;
        totalAscentTime = _elevatorPlatform.baseAscendTime + (_elevatorPlatform.weightTimeAddition * goldOnPlatformBalance);
        remainingTime = totalAscentTime;
    }

    private void HandleAscentEnd()
    {
        isAscending = false;
        UpdateTimeDisplay();
    }

    public void AddGold(int amount)
    {
        goldOnPlatformBalance += amount;
        UpdatePlatformWeight();
    }

    public void ResetWeight()
    {
        goldOnPlatformBalance = 0;
        UpdatePlatformWeight();
    }
    
    private void Update()
    {
        UpdatePlatformWeight();
        UpdateWeightArrowRotation();
     
        if (isAscending)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0)
                remainingTime = 0;
                
            UpdateTimeDisplay();
        }
    }

    private void UpdatePlatformWeight()
    {
        OnWeightChange?.Invoke();
        var weight = BaseWeight + goldOnPlatformBalance;
        _elevatorPlatform.platformWeight = weight;
    }
    
    private void UpdateWeightArrowRotation()
    {
        UpdateTimeDisplay();
        
        if (weightArrow == null) return;

        var rotationZ = goldOnPlatformBalance switch
        {
            0 => 90f,
            1 => 60f,
            2 => 30f,
            3 => 0f,
            4 => -30f,
            5 => -60f,
            _ => -90f
        };

        weightArrow.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
    
    public void UpdateTimeDisplay()
    {
        if (weightText == null) return;

        if (isAscending)
        {
            weightText.text = $"{remainingTime:F1}";
        }
        else
        {
            var time = _elevatorPlatform.baseAscendTime + (_elevatorPlatform.weightTimeAddition * goldOnPlatformBalance);
            weightText.text = $"{time}";
        }
    }
}