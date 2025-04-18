using System;
using GameControl;
using UnityEngine;
using TMPro;

public class PlatformWeight : MonoBehaviour
{
    [SerializeField] private GameObject weightArrow;
   
    
    private GoldManager _goldManager;
    private ElevatorPlatform _elevatorPlatform;
    
    private float _baseWeight = 1f; // Default platform weight

    private void Start()
    {
        _goldManager = G.GoldManager;
        _elevatorPlatform = G.ElevatorPlatform;
        
        if (weightArrow == null)
        {
            Debug.LogWarning("Weight arrow reference is missing! Please assign it in the inspector.");
        }
        
        
        // Initialize with base weight
        UpdatePlatformWeight();
    }

    private void Update()
    {
        UpdatePlatformWeight();
        UpdateWeightArrowRotation();
    }
    
    private void UpdatePlatformWeight()
    {
        // Set platform weight based on base weight + gold balance
        float weight = _baseWeight + _goldManager.GoldBalance;
        _elevatorPlatform.platformWeight = weight;
    }

    private void UpdateWeightArrowRotation()
    {
        if (weightArrow == null) return;
        
        float rotationZ = 90f; // Default rotation for balance = 0
        
        switch (_goldManager.GoldBalance)
        {
            case 0:
                rotationZ = 90f;
                break;
            case 1:
                rotationZ = 60f;
                break;
            case 2:
                rotationZ = 30f;
                break;
            case 3:
                rotationZ = 0f;
                break;
            case 4:
                rotationZ = -30f;
                break;
            case 5:
                rotationZ = -60f;
                break;
            default:
                // Balance >= 6
                rotationZ = -90f;
                break;
        }
        
        weightArrow.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
    
    
}