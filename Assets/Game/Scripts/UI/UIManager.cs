using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private void Awake()
    {
        G.UIManager = this;
    }

    // UI Screens
    private Dictionary<string, UIScreen> _screens = new Dictionary<string, UIScreen>();

    // Opened screens history
    private Stack<UIScreen> _screenHistory = new Stack<UIScreen>();

    // Current open screen
    private UIScreen _currentScreen;
    

    public void RegisterScreen(UIScreen screen)
    {
        if (!_screens.ContainsKey(screen.ScreenID))
        {
            _screens.Add(screen.ScreenID, screen);
            screen.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"Screen with ID {screen.ScreenID} already registered!");
        }
    }

    public void ShowScreen(string screenID, bool addToHistory = true)
    {
        if (_screens.TryGetValue(screenID, out var screen))
        {
            // Скрываем текущий экран, если он есть
            if (_currentScreen != null)
            {
                if (addToHistory)
                {
                    _screenHistory.Push(_currentScreen);
                }

                _currentScreen.Hide();
            }

            // Показываем новый экран
            screen.Show();
            _currentScreen = screen;
        }
        else
        {
            Debug.LogError($"Screen with ID {screenID} not found!");
        }
    }

    public void GoBack() // Метод для возврата к предыдущему экрану
    {
        if (_screenHistory.Count > 0)
        {
            UIScreen previousScreen = _screenHistory.Pop();
            ShowScreen(previousScreen.ScreenID, false);
        }
        else
        {
            Debug.LogWarning("No screens in history to go back to!");
        }
    }
}
        
        
   