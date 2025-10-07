using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletDisplay : MonoBehaviour
{
    [Header("Настройки отображения")]
    [SerializeField] private Wallet wallet; // Ссылка на кошелек
    [SerializeField] private TextMeshProUGUI balanceText; // UI элемент для отображения баланса

    private void Start()
    {
        wallet = Wallet.Instance;
        if (wallet == null || balanceText == null)
        {
            Debug.LogError("WalletDisplay: Не все ссылки установлены в инспекторе!");
            return;
        }

        // Подписываемся на событие изменения баланса
        wallet.OnBalanceChanged += UpdateBalanceDisplay;

        // Инициализируем отображение текущего баланса
        UpdateBalanceDisplay(wallet.GetBalance());
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        if (wallet != null)
        {
            wallet.OnBalanceChanged -= UpdateBalanceDisplay;
        }
    }

    // Обновление текста баланса
    private void UpdateBalanceDisplay(int newBalance)
    {
        balanceText.text = $"{newBalance}";
    }
}
