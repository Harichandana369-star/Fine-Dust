using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EpilogueManager : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI quoteText;

    [Header("Quote Content")]
    [TextArea(3, 5)]
    public string fullQuote = "Everything is temporary: the character you played, the world we live in, and the universe we belong to.";

    [Header("Timing Settings")]
    public float delayBeforeStart = 1.0f;  // Initial silence pause
    public float delayBetweenWords = 0.25f; // Speed: time in seconds per word
    public float stayDuration = 5.0f;      // How long full text stays visible

    [Header("Scene Transition Settings")]
    public bool returnToMainMenu = true;
    public string mainMenuSceneName = "MainMenu"; // Set to your menu scene name

    void Start()
    {
        if (quoteText != null)
        {
            quoteText.text = ""; // Clear text initially
        }

        StartCoroutine(RevealQuoteWordByWord());
    }

    private IEnumerator RevealQuoteWordByWord()
    {
        // 1. Initial pause in silence
        yield return new WaitForSeconds(delayBeforeStart);

        // 2. Split sentence into individual words
        string[] words = fullQuote.Split(' ');
        string currentText = "";

        // 3. Append words one by one
        for (int i = 0; i < words.Length; i++)
        {
            currentText += words[i] + " ";
            if (quoteText != null)
            {
                quoteText.text = currentText.TrimEnd();
            }

            yield return new WaitForSeconds(delayBetweenWords);
        }

        // 4. Keep full quote visible on screen
        yield return new WaitForSeconds(stayDuration);

        // 5. Load main menu or restart
        if (returnToMainMenu && !string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
