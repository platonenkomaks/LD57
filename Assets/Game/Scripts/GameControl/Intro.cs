using System;
using System.Collections;
using UnityEngine;

public class Intro : MonoBehaviour
{
    [Header("Flicker Settings")] 
    [SerializeField] private float flickerDuration = 1.0f;
    [SerializeField] private int flickerCount = 10;
    

    private GameObject _playerLight;
    private GameObject _hud;


    private bool _hasLanded = false;
    private RaycastHit _hit;

    public void Awake()
    {
        G.Intro = this;
    }

    public void Init(GameObject playerLight)
    {
        _playerLight = playerLight;
    }
    
    public IEnumerator StartIntro()
    {
        G.AudioManager.Play("Intro");
        G.AudioManager.Play("Neon");
        _playerLight.SetActive(false);
        
        yield return new WaitForSeconds(1f);
        G.AudioManager.Play("LightSwitch");
        _playerLight.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _playerLight.SetActive(false);
        
        
        yield return new WaitForSeconds(1f);
        G.AudioManager.Play("LightSwitch");
        _playerLight.SetActive(true);
        yield return new WaitForSeconds((0.2f));
        _playerLight.SetActive(false);
        
        var timePerFlicker = flickerDuration / flickerCount;

        for (int i = 0; i < flickerCount; i++)
        {
            var onRatio = Mathf.Lerp(0.1f, 0.8f, (float)i / flickerCount);
            var onTime = timePerFlicker * onRatio;
            var offTime = timePerFlicker - onTime;


            _playerLight.SetActive(true);
            yield return new WaitForSeconds(onTime);


            _playerLight.SetActive(false);
            yield return new WaitForSeconds(offTime);
        }
        
        G.AudioManager.Play("LightSwitch");
        G.AudioManager.Play("Landed");
        _playerLight.SetActive(true);
        
        yield return new WaitForSeconds(2f);
        
        G.HUD.gameObject.SetActive(true);
        
        
    }

  


    
}