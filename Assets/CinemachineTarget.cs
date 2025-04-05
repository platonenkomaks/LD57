using System;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineTarget : MonoBehaviour
{
    public CinemachineCamera _cinemachineCamera;

  

    public void SetTargetForCinemachineCamera(Transform player)
    {
        _cinemachineCamera.Follow = player;
    }
}
    
