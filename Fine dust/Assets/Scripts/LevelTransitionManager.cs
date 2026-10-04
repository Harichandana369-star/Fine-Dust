using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTransitionManager : MonoBehaviour
{
    public static LevelTransitionManager Instance;

    [Header("UI Reference")]
    public Image fadeImage;

    [Header("Transition Timings")]
    public float fadeOutDuration = 0.6f;  // Time it takes to fade to black
    public float darkPauseDuration = 1.0f; // Time spent staying completely black between levels
    public float fadeInDuration = 0.6f;   // Time it takes to fade in to the new level

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
        // Automatically fade in when a new scene finishes loading
        if (fadeImage != null)
        {
            StartCoroutine(FadeFromBlack());
        }
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(FadeAndLoadScene(nextSceneIndex));
    }

    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(FadeAndLoadSceneByName(sceneName));
    }

    private IEnumerator FadeToBlack()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / fadeOutDuration);
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

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (timer / fadeInDuration));
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
    }

    private IEnumerator FadeAndLoadScene(int sceneIndex)
    {
        // 1. Fade screen out to black
        yield return StartCoroutine(FadeToBlack());

        // 2. Pause in complete darkness (Adjust darkPauseDuration in Inspector!)
        yield return new WaitForSeconds(darkPauseDuration);

        // 3. Load the new level while dark
        if (sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0); // Reset to Main Menu / Level 1
        }
    }

    private IEnumerator FadeAndLoadSceneByName(string sceneName)
    {
        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(darkPauseDuration);
        SceneManager.LoadScene(sceneName);
    }
}
