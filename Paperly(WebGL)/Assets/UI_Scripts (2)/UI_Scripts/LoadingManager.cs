using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void LoadSceneWithDelay(string sceneName, float delay)
    {
        StartCoroutine(LoadRoutine(sceneName, delay));
    }

    private IEnumerator LoadRoutine(string sceneName, float delay)
    {
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene(sceneName);
    }
}
