using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private float minimumLoadingTime = 0.5f;

    // Событие, которое вызывается перед началом загрузки сцены
    public event Action<string> OnBeforeSceneLoad;

    // Событие, которое вызывается после завершения загрузки сцены
    public event Action<string> OnAfterSceneLoad;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        
        G.SceneLoader = this;
        if (loadingScreen) loadingScreen.SetActive(false);
    }

    // Загрузка сцены по имени
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    // Перезагрузка текущей сцены
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        LoadScene(currentScene);
    }

    // Загрузка следующей сцены в билде
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        StartCoroutine(LoadSceneAsync(nextSceneIndex));
    }

    // Асинхронная загрузка сцены по имени
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Вызываем событие перед загрузкой
        OnBeforeSceneLoad?.Invoke(sceneName);

        // Показываем экран загрузки
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // Запоминаем время начала загрузки
        float startTime = Time.time;

        // Начинаем асинхронную загрузку сцены
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // Запрещаем автоматическую активацию сцены
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // Обновляем прогресс-бар (asyncOperation.progress дает значение 0-0.9)
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            if (progressText != null)
            {
                progressText.text = $"{Mathf.Round(progress * 100)}%";
            }

            // Когда загрузка почти завершена
            if (asyncOperation.progress >= 0.9f)
            {
                // Проверяем, прошло ли минимальное время загрузки
                if (Time.time - startTime >= minimumLoadingTime)
                {
                    // Активируем сцену
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        // Скрываем экран загрузки
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }

        // Вызываем событие после загрузки
        OnAfterSceneLoad?.Invoke(sceneName);
    }

    // Асинхронная загрузка сцены по индексу
    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        // Аналогично LoadSceneAsync(string), но с индексом
        OnBeforeSceneLoad?.Invoke($"Scene {sceneIndex}");

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        float startTime = Time.time;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            if (progressText != null)
            {
                progressText.text = $"{Mathf.Round(progress * 100)}%";
            }

            if (asyncOperation.progress >= 0.9f && Time.time - startTime >= minimumLoadingTime)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }

        OnAfterSceneLoad?.Invoke($"Scene {sceneIndex}");
    }
}