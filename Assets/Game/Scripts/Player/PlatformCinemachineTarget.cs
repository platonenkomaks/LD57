using System.Collections;
using Events;
using UnityEngine;

public class PlatformCinemachineTarget : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        G.EventManager.Register<OnPlatformEnter>(FocusOnPlatform);
        G.EventManager.Register<OnPlatformExit>(FocusOnPlayer);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        G.EventManager.Unregister<OnPlatformEnter>(FocusOnPlatform);
        G.EventManager.Unregister<OnPlatformExit>(FocusOnPlayer);
    }

    private void FocusOnPlatform(OnPlatformEnter e)
    {
        gameObject.SetActive(true);
    }

    private void FocusOnPlayer(OnPlatformExit e)
    {
        gameObject.SetActive(false);
    }
}