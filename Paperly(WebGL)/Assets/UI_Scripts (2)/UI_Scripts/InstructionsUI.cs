
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InstructionsUI : MonoBehaviour
{
    [SerializeField] private Button okayButton;
  //  [SerializeField] private Button backButtonButton;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject tabLoadingScreen;

    private void Start()
    {
        okayButton.onClick.AddListener(OnOkayClick);
    }

    private void OnOkayClick()
    {
        StartCoroutine(LoadLevelWithDelay());
    }

    private IEnumerator LoadLevelWithDelay()
    {

        loadingScreen.SetActive(true);
      
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(1);
    }
}
