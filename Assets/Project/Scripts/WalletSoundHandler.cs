using UnityEngine;

public class WalletSoundHandler : MonoBehaviour
{
    [Header("Звук изменения баланса")]
    [SerializeField] private AudioSource balanceChangeSound;

    public static WalletSoundHandler Instance { get; private set; } // Синглтон

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Уничтожаем дубликат синглтона
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Сохраняем объект между сценами
    }

    private void Start()
    {
        if (Wallet.Instance != null)
        {
            Wallet.Instance.OnBalanceChanged += PlayBalanceChangeSound;
        }
    }

    private void PlayBalanceChangeSound(int newBalance)
    {
        Debug.Log(newBalance);
        if (balanceChangeSound != null)
        {
            balanceChangeSound.Play();
        }
    }
}
