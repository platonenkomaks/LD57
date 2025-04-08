using UnityEngine;
using System;

public abstract class UIScreen : MonoBehaviour
{
    [SerializeField] private string screenID;
    public string ScreenID => screenID;
    
    public event Action OnScreenShown;
    public event Action OnScreenHidden;

    protected virtual void Start()
    {
        G.UIManager.RegisterScreen(this);
    }
    
    protected virtual void OnDestroy()
    {
        G.UIManager.DeregisterScreen(this);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);

        // Animation of the screen opening

        OnShown();
    }

    public virtual void Hide()
    {
        // Animation of the screen closing

        gameObject.SetActive(false);

        OnHidden();
    }

    protected virtual void OnShown()
    {
        OnScreenShown?.Invoke();
    }

    protected virtual void OnHidden()
    {
        OnScreenHidden?.Invoke();
    }

    
}