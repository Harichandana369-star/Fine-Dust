using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class HeartEnding : MonoBehaviour
{
    [Header("UI & Prompt References")]
    public GameObject interactPrompt;     // Drag InteractPrompt UI text here

    [Header("VFX & Lighting")]
    public ParticleSystem dustBurstPrefab; // Drag PFX_ChalkDust prefab here
    public Light2D heartLight;            // Drag 2D Point Light child here

    [Header("Repair & Transition Settings")]
    public float drainRate = 45f;         // Speed mass drains while holding 'A'
    public float delayBeforeEpilogue = 1.8f; // Delay after player dissolves before changing scene

    private bool isPlayerInZone = false;
    private bool isEndingTriggered = false;
    private ChalkPlayer playerScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ChalkPlayer player = collision.GetComponent<ChalkPlayer>();
        if (player != null)
        {
            isPlayerInZone = true;
            playerScript = player;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<ChalkPlayer>() != null)
        {
            isPlayerInZone = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (isEndingTriggered || !isPlayerInZone || playerScript == null) return;

        // Player holds 'A' to repair the broken heart using their body mass
        if (Input.GetKey(KeyCode.A))
        {
            // 1. Drain player mass rapidly
            playerScript.ConsumeMass(drainRate * Time.deltaTime);

            // 2. Pulsate neon pink light intensity during repair
            if (heartLight != null)
            {
                heartLight.intensity = 3.5f + Mathf.PingPong(Time.time * 6f, 2.5f);
            }

            // 3. Once mass drops to minimum/zero, complete dissolution & switch scenes
            if (playerScript.currentMass <= playerScript.minMass + 0.5f)
            {
                StartCoroutine(SequenceEndingAndTransition());
            }
        }
    }

    private IEnumerator SequenceEndingAndTransition()
    {
        isEndingTriggered = true;

        if (interactPrompt != null) interactPrompt.SetActive(false);

        // 1. Spawn fine chalk dust particle explosion at player location
        if (dustBurstPrefab != null)
        {
            Instantiate(dustBurstPrefab, playerScript.transform.position, Quaternion.identity);
        }

        // 2. Destroy player character object
        Destroy(playerScript.gameObject);

        // 3. Pause briefly in silence as dust particles float away
        yield return new WaitForSeconds(delayBeforeEpilogue);

        // 4. Load the dedicated Epilogue Scene for the quote
        SceneManager.LoadScene("EpilogueScene");
    }
}
