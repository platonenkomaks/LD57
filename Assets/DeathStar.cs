using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class DeathStar : MonoBehaviour
{
    private Light2D _light2D;
    [SerializeField] private float flashDuration = 2.0f; // Продолжительность вспышки в секундах
    [SerializeField] private float maxIntensity = 150.0f; // Максимальная интенсивность света
    
    
    private void Start()
    {
        G.DeathStar = this;
        _light2D = GetComponent<Light2D>();
        _light2D.enabled = false;
        _light2D.intensity = 0f;
        
        // Другие настройки будем менять через инспектор
    }
    
    public void StartDestroy()
    {
        G.HUD.Hide();
        _light2D.enabled = true;
        G.AudioManager.Play("Fire");
        StartCoroutine(FlashEffect());
    }
    
    private IEnumerator FlashEffect()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < flashDuration)
        {
            // Плавно увеличиваем интенсивность света от 0 до maxIntensity
            _light2D.intensity = Mathf.Lerp(0f, maxIntensity, elapsedTime / flashDuration);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Устанавливаем окончательную интенсивность на максимум для уверенности
        _light2D.intensity = maxIntensity;
    }
    
    public void StopDestroy()
    {
        G.HUD.Show();
        _light2D.intensity = 0f;
        _light2D.enabled = false;
        
    }
}