using UnityEngine;

public class Cog : MonoBehaviour
{
    public float baseRotationSpeed = 1f;  // Базовая скорость вращения
    public Vector3 rotationAxisForward = Vector3.forward;
    public Vector3 rotationAxisBack = Vector3.back;
    
    private bool isRotating = false;
    private Vector3 currentRotationAxis;
    private float currentRotationSpeed;

    private void Update()
    {
        if (isRotating)
        {
            transform.Rotate(currentRotationAxis, currentRotationSpeed * Time.deltaTime);
        }
    }

    public void StartRotation(bool isAscending, float platformSpeed)
    {
        isRotating = true;
        currentRotationAxis = isAscending ? rotationAxisForward : rotationAxisBack;
        
        // Скорость вращения пропорциональна скорости платформы
        currentRotationSpeed = baseRotationSpeed * (platformSpeed / 0.4f);  
    }
    
    public void StopRotation()
    {
        isRotating = false;
    }
}