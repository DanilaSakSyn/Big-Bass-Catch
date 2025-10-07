using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [Header("Настройки кошелька")]
    [SerializeField] private int initialBalance = 0; // Начальный баланс

    private const string WalletKey = "WalletBalance"; // Ключ для сохранения прогресса
    private int currentBalance;

    public event Action<int> OnBalanceChanged; // Событие для уведомления об изменении баланса

    public static Wallet Instance { get; private set; } // Синглтон

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Уничтожаем дубликат синглтона
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Сохраняем объект между сценами

        LoadProgress();
    }

    // Метод для добавления монет
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        currentBalance += amount;
        SaveProgress();
        Debug.Log("Current Balance: " + currentBalance);
        OnBalanceChanged?.Invoke(currentBalance);
    }

    // Метод для снятия монет
    public bool SpendCoins(int amount)
    {
        if (amount < 0 || amount > currentBalance) return false;

        currentBalance -= amount;
        SaveProgress();
        OnBalanceChanged?.Invoke(currentBalance);
        return true;
    }

    // Получение текущего баланса
    public int GetBalance()
    {
        return currentBalance;
    }

    // Сохранение прогресса
    private void SaveProgress()
    {
        PlayerPrefs.SetInt(WalletKey, currentBalance);
        PlayerPrefs.Save();
    }

    // Загрузка прогресса
    private void LoadProgress()
    {
        currentBalance = PlayerPrefs.GetInt(WalletKey, initialBalance);
        OnBalanceChanged?.Invoke(currentBalance);
    }

    // Сброс прогресса (для тестирования или отладки)
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(WalletKey);
        currentBalance = initialBalance;
        OnBalanceChanged?.Invoke(currentBalance);
    }

}
