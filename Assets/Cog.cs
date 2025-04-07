using UnityEngine;

public class Cog : MonoBehaviour
{
    public float rotationSpeed = 10f;
  
    public Vector3 rotationAxis = Vector3.forward;
    
    private bool _isRotating = false;

    private void Update()
    {
        if (_isRotating)
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }
    }
    

    public void StartRotation()
    {
        _isRotating = true;
    }
    
    
    public void StopRotation()
    {
        _isRotating = false;
    }
    
}
