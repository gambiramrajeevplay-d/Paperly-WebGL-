using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckpointManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject gameOverPanel;
    public GameObject levelGameObject;
    public TextMeshProUGUI checkpointText;

    [Header("Settings")]
    public int maxMisses = 3;

    [Header("Reverse Settings")]
    public float reverseSpeed = 40f;
    public float reverseDuration = 5f;

    [Header("Audio")]
    public AudioClip checkpointSound;
    private AudioSource audioSource;

    private int expectedCheckpoint = 0;
    private int missCount = 0;
    private bool isReversing = false;

    private List<CheckPoint> checkpoints = new List<CheckPoint>();

    // ================= OBSERVER =================

    void OnEnable()
    {
        CheckPoint.OnCheckpointCollected += PlayerHitCheckpoint;
    }

    void OnDisable()
    {
        CheckPoint.OnCheckpointCollected -= PlayerHitCheckpoint;
    }

    void Start()
    {
        // 🔥 Get player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("No GameObject found with tag 'Player'");

        // 🔊 Get AudioSource from object tagged "PickUp"
        GameObject audioObj = GameObject.FindGameObjectWithTag("PickUp");
        if (audioObj != null)
            audioSource = audioObj.GetComponent<AudioSource>();

        // 🔥 Auto find all checkpoints
        CheckPoint[] all = FindObjectsOfType<CheckPoint>();

        foreach (CheckPoint cp in all)
        {
            checkpoints.Add(cp);
        }

        // 🔥 Sort checkpoints by index
        checkpoints.Sort((a, b) => a.checkpointIndex.CompareTo(b.checkpointIndex));
    }

    void Update()
    {
        if (isReversing)
            return;

        if (expectedCheckpoint < checkpoints.Count)
        {
            CheckPoint nextCP = checkpoints[expectedCheckpoint];

            if (!nextCP.IsCollected())
            {
                if (player.position.z > nextCP.transform.position.z + 5f)
                {
                    Fail();
                }
            }
        }
    }


    // ================= CHECKPOINT EVENT =================

    public void PlayerHitCheckpoint(int index)
    {
        if (isReversing)
            return;

        if (index == expectedCheckpoint)
        {
            Success();
            expectedCheckpoint++;
        }
        else if (index > expectedCheckpoint)
        {
            Fail();
        }
    }

    void Success()
    {
        missCount = 0;

        if (checkpointText != null)
        {
            checkpointText.text = "Checkpoint Collected!";
            checkpointText.gameObject.SetActive(true);
            StartCoroutine(HideText());
        }

        if (checkpointSound != null && audioSource != null)
            audioSource.PlayOneShot(checkpointSound);
    }

    void Fail()
    {
        if (isReversing)
            return;

        missCount++;

        if (missCount >= maxMisses)
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            if (levelGameObject != null)
                levelGameObject.SetActive(false);

            return;
        }

        StartCoroutine(ReverseGameplay());
    }

    IEnumerator HideText()
    {
        yield return new WaitForSeconds(2f);

        if (checkpointText != null)
            checkpointText.gameObject.SetActive(false);
    }

    IEnumerator ReverseGameplay()
    {
        isReversing = true;

        PlaneController plane = player.GetComponent<PlaneController>();
        if (plane != null)
            plane.canControl = false;

        Rigidbody rb = player.GetComponent<Rigidbody>();

        float timer = 0f;

        while (timer < reverseDuration)
        {
            timer += Time.deltaTime;

            if (rb != null)
            {
                Vector3 vel = rb.velocity;
                vel.z = -reverseSpeed;
                rb.velocity = vel;
            }

            yield return null;
        }

        if (plane != null)
            plane.canControl = true;

        isReversing = false;
    }

    public void ForceReverseWithMessage(string message)
    {
        if (isReversing)
            return;

        if (checkpointText != null)
        {
            checkpointText.text = message;
            checkpointText.gameObject.SetActive(true);
            StartCoroutine(HideText());
        }

        StartCoroutine(ReverseGameplay());
    }

}
