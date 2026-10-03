using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class HeartEnding : MonoBehaviour
{
    public GameObject interactPrompt;
    public ParticleSystem dustBurstPrefab;
    public Light2D heartLight;

    public float drainRate = 45f;
    public float delayBeforeEpilogue = 1.8f;

    private bool isPlayerInZone = false;
    private bool isEndingTriggered = false;
    private ChalkPlayer playerScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Something touched the heart: " + collision.name);

        ChalkPlayer player = collision.GetComponent<ChalkPlayer>();
        if (player != null)
        {
            Debug.Log("Player entered Heart trigger zone!");
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

        if (Input.GetKey(KeyCode.E))
        {
            Debug.Log("E key is being held down! Current Mass: " + playerScript.currentMass);

            playerScript.ConsumeMass(drainRate * Time.deltaTime);

            if (heartLight != null)
            {
                heartLight.intensity = 3.5f + Mathf.PingPong(Time.time * 6f, 2.5f);
            }

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

        if (dustBurstPrefab != null)
        {
            Instantiate(dustBurstPrefab, playerScript.transform.position, Quaternion.identity);
        }

        Destroy(playerScript.gameObject);

        yield return new WaitForSeconds(delayBeforeEpilogue);

        SceneManager.LoadScene("EpilogueScene");
    }
}
