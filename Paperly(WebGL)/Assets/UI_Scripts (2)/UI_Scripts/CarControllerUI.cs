using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CarControllerUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button[] buyButtons;     // Buy buttons (locked cars)
    [SerializeField] private Button[] selectButtons;  // Select buttons (unlocked cars)

    [Header("Car Costs (Match Order)")]
    [SerializeField] private int[] carCosts;

    [Header("Button Sprites")]
    [SerializeField] private Sprite selectSprite;
    [SerializeField] private Sprite inUseSprite;

    [Header("Currency UI")]
    [SerializeField] private TMP_Text coinsText;

    [Header("Not Enough Coins Popup")]
    [SerializeField] private GameObject notEnoughCoinsPopup;
    [SerializeField] private float popupDuration = 2f;

    private int selectedCarIndex = 0;

    // =========================
    // UNITY
    // =========================

    void Start()
    {
        UnlockFirstCarIfNeeded();

        selectedCarIndex = PlayerPrefs.GetInt("selectedCar", 0);

        UpdateCoinsUI();
        UpdateAllUI();

        if (CurrencyManager.instance != null)
            CurrencyManager.instance.OnCurrencyChanged += OnCurrencyChanged;
    }

    void OnDestroy()
    {
        if (CurrencyManager.instance != null)
            CurrencyManager.instance.OnCurrencyChanged -= OnCurrencyChanged;
    }

    // =========================
    // FIRST CAR UNLOCK
    // =========================

    void UnlockFirstCarIfNeeded()
    {
        if (CurrencyManager.instance == null) return;

        if (!CurrencyManager.instance.IsCharacterUnlocked(0))
        {
            CurrencyManager.instance.UnlockCharacter(0);
        }

        if (!PlayerPrefs.HasKey("selectedCar"))
        {
            PlayerPrefs.SetInt("selectedCar", 0);
            PlayerPrefs.Save();
        }
    }

    // =========================
    // CURRENCY
    // =========================

    void OnCurrencyChanged(int amount)
    {
        UpdateCoinsUI();
    }

    void UpdateCoinsUI()
    {
        if (coinsText != null && CurrencyManager.instance != null)
            coinsText.text = CurrencyManager.instance.GetCurrency().ToString();
    }

    // =========================
    // SELECT
    // =========================

    public void SelectCar(int index)
    {
        if (CurrencyManager.instance == null) return;
        if (index < 0 || index >= selectButtons.Length) return;

        if (!CurrencyManager.instance.IsCharacterUnlocked(index))
            return;

        selectedCarIndex = index;
        PlayerPrefs.SetInt("selectedCar", index);
        PlayerPrefs.Save();

        UpdateAllUI();
    }

    // =========================
    // BUY
    // =========================

    public void BuyCar(int index)
    {
        Debug.Log("BuyCar clicked. Index: " + index);

        if (CurrencyManager.instance == null)
        {
            Debug.LogError("CurrencyManager NULL");
            return;
        }

        if (index < 0 || index >= carCosts.Length)
        {
            Debug.LogError($"Invalid index: {index} | carCosts length: {carCosts.Length}");
            return;
        }

        int cost = carCosts[index];
        int currentCoins = CurrencyManager.instance.GetCurrency();

        Debug.Log("Cost: " + cost + " Coins: " + currentCoins);

        if (currentCoins < cost)
        {
            Debug.Log("Not enough coins");
            ShowNotEnoughCoins();
            return;
        }

        Debug.Log("Unlocking car...");
        CurrencyManager.instance.SpendCurrency(cost);
        CurrencyManager.instance.UnlockCharacter(index);

        UpdateAllUI();
    }



    // =========================
    // UI UPDATE
    // =========================

    void UpdateAllUI()
    {
        int totalCars = Mathf.Max(buyButtons.Length, selectButtons.Length);

        for (int i = 0; i < totalCars; i++)
        {
            bool unlocked = CurrencyManager.instance != null &&
                            CurrencyManager.instance.IsCharacterUnlocked(i);

            // BUY BUTTON
            if (i < buyButtons.Length && buyButtons[i] != null)
            {
                buyButtons[i].gameObject.SetActive(!unlocked);
            }

            // SELECT BUTTON
            if (i < selectButtons.Length && selectButtons[i] != null)
            {
                selectButtons[i].gameObject.SetActive(unlocked); // OFF if not bought

                if (unlocked)
                {
                    Image img = selectButtons[i].GetComponent<Image>();
                    if (img != null)
                    {
                        img.sprite = (i == selectedCarIndex)
                            ? inUseSprite
                            : selectSprite;
                    }
                }
            }
        }
    }

    // =========================
    // POPUP
    // =========================

    void ShowNotEnoughCoins()
    {
        StopAllCoroutines();
        StartCoroutine(NotEnoughRoutine());
    }

    IEnumerator NotEnoughRoutine()
    {
        if (notEnoughCoinsPopup == null) yield break;

        notEnoughCoinsPopup.SetActive(true);
        yield return new WaitForSeconds(popupDuration);
        notEnoughCoinsPopup.SetActive(false);
    }
}
