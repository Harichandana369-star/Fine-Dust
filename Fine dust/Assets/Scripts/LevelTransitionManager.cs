using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTransitionManager : MonoBehaviour
{
    public static LevelTransitionManager Instance;

    [Header("UI Reference")]
    public Image fadeImage;

    [Header("Transition Settings")]
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            StartCoroutine(FadeFromBlack());
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Automatically fade in whenever a new scene loads
        if (fadeImage != null)
        {
            StartCoroutine(FadeFromBlack());
        }
    }

    // Call this method from your Goal / Finish line script to switch levels
    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(FadeAndLoadScene(nextSceneIndex));
    }

    // Call this method to load a specific scene by name
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(FadeAndLoadSceneByName(sceneName));
    }

    private IEnumerator FadeToBlack()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    private IEnumerator FadeFromBlack()
    {
        float timer = 0f;
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (timer / fadeDuration));
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
    }

    private IEnumerator FadeAndLoadScene(int sceneIndex)
    {
        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator FadeAndLoadSceneByName(string sceneName)
    {
        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene(sceneName);
    }
}
