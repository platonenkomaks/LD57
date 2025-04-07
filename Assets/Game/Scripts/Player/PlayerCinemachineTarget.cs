using System.Collections;
using Events;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCinemachineTarget : MonoBehaviour
{
    private CinemachineCamera _cmCamera;

    private void Awake()
    {
        _cmCamera = GetComponent<CinemachineCamera>();
    }
    
    private IEnumerator Start()
    {
        yield return null;
        G.EventManager.Register<OnPlatformEnter>(FocusOnPlatform);
        G.EventManager.Register<OnPlatformExit>(FocusOnPlayer);
        
    }

    private void OnDestroy()
    {
        G.EventManager.Unregister<OnPlatformEnter>(FocusOnPlatform);
        G.EventManager.Unregister<OnPlatformExit>(FocusOnPlayer);
    }

    private void FocusOnPlatform(OnPlatformEnter e)
    {
        gameObject.SetActive(false);
    }

    private void FocusOnPlayer(OnPlatformExit e)
    {
        gameObject.SetActive(true);
    }
    
    public void SetTargetForCinemachineCamera(Transform newPlayer)
    {
        _cmCamera.Follow = newPlayer;
    }
}
    
