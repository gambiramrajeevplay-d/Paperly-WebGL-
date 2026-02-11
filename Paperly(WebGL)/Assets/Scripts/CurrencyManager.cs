using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour

{

    public static CurrencyManager instance;

    public event Action<int> OnCurrencyChanged;

    [Header("Currency")]
    [SerializeField] private int currentAmount;

    private const string COIN_KEY = "TotalCoins";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        currentAmount = PlayerPrefs.GetInt(COIN_KEY, 0);
    }

    // ---------------- COINS ----------------

    public void AddCurrency(int amount)
    {
        if (amount <= 0) return;

        currentAmount += amount;
        Save();
    }

    public bool SpendCurrency(int amount)
    {
        if (currentAmount < amount)
            return false;

        currentAmount -= amount;
        Save();
        return true;
    }

    public int GetCurrency()
    {
        return currentAmount;
    }

    private void Save()
    {
        PlayerPrefs.SetInt(COIN_KEY, currentAmount);
        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke(currentAmount);
    }

    // ---------------- UNLOCKS ----------------

    public void UnlockCharacter(int characterIndex)
    {
        PlayerPrefs.SetInt("CharUnlocked_" + characterIndex, 1);
        //  PlayerPrefs.SetInt("character", presentChar);
        PlayerPrefs.Save();
    }

    public bool IsCharacterUnlocked(int characterIndex)
    {
        return PlayerPrefs.GetInt("CharUnlocked_" + characterIndex, 0) == 1;
    }

}


