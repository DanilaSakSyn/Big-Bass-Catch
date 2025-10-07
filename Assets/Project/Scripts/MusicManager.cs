using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; } // Синглтон

    [Header("Настройки музыки")]
    [SerializeField] private List<AudioClip> musicTracks; // Список музыкальных треков
    [SerializeField] private AudioSource audioSource; // Аудио источник для воспроизведения

    private int currentTrackIndex = 0;

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
        if (musicTracks.Count > 0 && audioSource != null)
        {
            StartCoroutine(PlayMusicLoop());
        }
    }

    private IEnumerator PlayMusicLoop()
    {
        while (true)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = musicTracks[currentTrackIndex];
                audioSource.Play();

                currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Count; // Переход к следующему треку
            }

            yield return null; // Ждем следующий кадр
        }
    }
}
