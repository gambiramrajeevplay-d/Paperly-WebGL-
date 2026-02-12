using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreMenu_UI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button closeButton;

    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;

    [Header("Currency")]
    [SerializeField] private Text currencyText;

    private CurrencyManager currecnyManager;
    private Animator animator;

    private void OnEnable()
    {
        if(currecnyManager != null)
        {
            UpdateCurrenyText();
        }

        if (animator != null)
        {
            animator.SetTrigger("Entry");
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        closeButton.onClick.AddListener(OpenMainMenu);

        currecnyManager = CurrencyManager.instance;
        currecnyManager.OnCurrencyChanged += CurrecnyManager_OnCurrencyChanged;

        UpdateCurrenyText();
    }

    private void CurrecnyManager_OnCurrencyChanged(int newAmount)
    {
        currencyText.text = newAmount.ToString();
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
           // OpenMainMenu();
        }
    }

    public void OpenMainMenu()
    {
        if(SaveScript.hasNoticeUI)
        {
            return;
        }

        gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void UpdateCurrenyText()
    {
        currencyText.text = currecnyManager.GetCurrency().ToString();
    }
}
