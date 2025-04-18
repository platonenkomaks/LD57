using System;
using GameControl;
using UnityEngine;
using TMPro;

public class PlatformWeight : MonoBehaviour
{
    [SerializeField] private GameObject weightArrow;

    public int goldOnPlatformBalance;

    private ElevatorPlatform _elevatorPlatform;

    private const float BaseWeight = 1f; // Default platform weight

    private void Start()
    {
        goldOnPlatformBalance = 0;

        _elevatorPlatform = G.ElevatorPlatform;
        
        UpdatePlatformWeight();
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
    }

    private void UpdatePlatformWeight()
    {
        var weight = BaseWeight + goldOnPlatformBalance;
        _elevatorPlatform.platformWeight = weight;
    }
    

    private void UpdateWeightArrowRotation()
    {
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
}