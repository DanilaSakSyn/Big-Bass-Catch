using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioToggleController : MonoBehaviour
{
    [Header("Настройки AudioMixer")]
    [SerializeField] private AudioMixer audioMixer; // Ссылка на AudioMixer
    [SerializeField] private string volumeParameter = "MasterVolume"; // Параметр громкости в AudioMixer
    [SerializeField] private Toggle muteToggle; // Ссылка на Toggle для управления звуком

    private const float MuteVolume = -80f; // Значение громкости для отключения звука
    private const float UnmuteVolume = 0f; // Значение громкости для включения звука

    private void Start()
    {
        if (muteToggle != null)
        {
            muteToggle.onValueChanged.AddListener(OnToggleValueChanged);
            // Устанавливаем начальное состояние Toggle в соответствии с текущим состоянием звука
            float currentVolume;
            audioMixer.GetFloat(volumeParameter, out currentVolume);
            muteToggle.isOn = currentVolume > MuteVolume;
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(volumeParameter, isOn ? UnmuteVolume : MuteVolume);
        }
    }

    private void OnDestroy()
    {
        if (muteToggle != null)
        {
            muteToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}
