using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelCoinManager : MonoBehaviour
{
    public static LevelCoinManager instance;

    [Header("Level Coins")]
    private int collectedCoins;

    [Header("UI (Optional)")]
    [SerializeField] private TMP_Text levelCoinText;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        //DontDestroyOnLoad(gameObject);   // 🔥 IMPORTANT

        collectedCoins = 0;
        UpdateUI();
    }


    public void AddCoin(int amount)
    {
        collectedCoins += amount;
        UpdateUI();
    }

    public int GetCollectedCoins()
    {
        return collectedCoins;
    }

    public void ResetCoins()
    {
        collectedCoins = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (levelCoinText)
            levelCoinText.text = "Coins : " + collectedCoins;
    }

}
