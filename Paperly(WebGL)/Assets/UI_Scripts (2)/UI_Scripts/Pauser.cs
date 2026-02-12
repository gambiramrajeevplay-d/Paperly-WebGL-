
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pauser : MonoBehaviour
{
    public static Pauser instance;

    [Header("UI")]
    public GameObject PausePannel;
    public GameObject LevelObject;
    public GameObject PauseButton;
    public GameObject levelFailPanel;

    // 🔒 GLOBAL PAUSE LOCK (Boss fight, cutscenes, etc.)
    public static bool PauseLocked = false;

    private void Awake()
    {
        instance = this;
        AudioManagerPause.Initialize();
    }

    private void OnEnable()
    {
        //if (AndroidTV.IsAndroidOrFireTv())
        //    PauseButton.SetActive(false);
        //else
        //    PauseButton.SetActive(true);
    }

    private void Start()
    {
        //if (AndroidTV.IsAndroidOrFireTv())
        //    PauseButton.SetActive(false);
        //else
        //    PauseButton.SetActive(true);
    }

    void Update()
    {
        // 🔒 HARD BLOCK pause when locked
        if (PauseLocked)
        return;
        

        // ❌ Already finished → no pause
        //if (FinishLine.instance.levelPassPanel.activeSelf || levelFailPanel.activeSelf)
          //  return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        // 🔒 BLOCK UI pause
        if (PauseLocked)
            return;

        LevelObject.SetActive(false);
        PausePannel.SetActive(true);
        // PauseButton?.SetActive(false);

       // if (!AndroidTV.IsAndroidOrFireTv())
       //     PauseButton.SetActive(false);
        
        Time.timeScale = 0f;
        UpdateSoundSprite();
    }

    public void Resume()
    {
        LevelObject.SetActive(true);
        PausePannel.SetActive(false);
      //  PauseButton?.SetActive(true);

       // if (!AndroidTV.IsAndroidOrFireTv())
            PauseButton.SetActive(true);

        Time.timeScale = 1f;
    }

    public void MM()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("UI");
    }

    public void Mute()
    {
        AudioManagerPause.IsMuted = !AudioManagerPause.IsMuted;
        UpdateSoundSprite();
    }

    private void UpdateSoundSprite()
    {
        // sound.sprite = AudioManagerPause.IsMuted ? soundoff : soundon;
    }

    // 🔥 CALLED FROM BOSS TRIGGER
    public static void LockPause()
    {
        PauseLocked = true;

        // force unpause
        if (instance != null)
        {
            instance.PausePannel.SetActive(false);
            instance.LevelObject.SetActive(true);
        }
    }

    public static void UnlockPause()
    {
        PauseLocked = false;
    }
}
