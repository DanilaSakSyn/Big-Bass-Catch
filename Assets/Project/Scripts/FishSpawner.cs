using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    [SerializeField] private GameObject[] fishPrefabs;           // Массив префабов рыб
    [SerializeField] private float spawnInterval = 2f;          // Интервал между спавнами
    [SerializeField] private float minSpawnInterval = 1f;       // Минимальный интервал
    [SerializeField] private float maxSpawnInterval = 4f;       // Максимальный интервал
    
    [Header("Позиционирование")]
    [SerializeField] private float spawnDistanceFromScreen = 2f; // Расстояние за экраном для спавна
    [SerializeField] private float minY = -3f;                  // Минимальная Y позиция для спавна
    [SerializeField] private float maxY = 3f;                   // Максимальная Y позиция для спавна
    
    [Header("Скорость рыб")]
    [SerializeField] private float minFishSpeed = 1f;           // Минимальная скорость рыбы
    [SerializeField] private float maxFishSpeed = 4f;           // Максимальная скорость рыбы
    
    [Header("Вероятность направления")]
    [Range(0f, 1f)]
    [SerializeField] private float rightDirectionChance = 0.5f; // Шанс спавна рыбы справа налево
    
    private Camera mainCamera;
    private float leftSpawnX;
    private float rightSpawnX;
    private bool isSpawning = false;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CalculateSpawnBounds();
            StartSpawning();
        }
        else
        {
            Debug.LogError("FishSpawner: Главная камера не найдена!");
        }
    }

    private void CalculateSpawnBounds()
    {
        // Рассчитываем позиции спавна за пределами экрана
        Vector3 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        
        leftSpawnX = -screenBounds.x - spawnDistanceFromScreen;   // Левая граница спавна
        rightSpawnX = screenBounds.x + spawnDistanceFromScreen;   // Правая граница спавна
    }

    public void StartSpawning()
    {
        if (!isSpawning && fishPrefabs.Length > 0)
        {
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnFish();
            
            // Случайный интервал между спавнами
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnFish()
    {
        if (fishPrefabs.Length == 0) return;

        // Выбираем случайный префаб рыбы
        GameObject fishPrefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
        
        // Определяем направление движения рыбы
        bool moveRight = Random.value > rightDirectionChance;
        
        // Определяем позицию спавна
        Vector3 spawnPosition = GetSpawnPosition(moveRight);
        
        // Создаем рыбу
        GameObject newFish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
        
        // Настраиваем компонент Fish
        Fish fishComponent = newFish.GetComponent<Fish>();
        if (fishComponent == null)
        {
            fishComponent = newFish.AddComponent<Fish>();
        }
        
        // Устанавливаем направление и скорость
        fishComponent.SetDirection(moveRight);
        fishComponent.SetSpeed(Random.Range(minFishSpeed, maxFishSpeed));
    }

    private Vector3 GetSpawnPosition(bool moveRight)
    {
        float spawnX;
        float spawnY = Random.Range(minY, maxY);
        
        if (moveRight)
        {
            // Спавним слева, рыба плывет вправо
            spawnX = leftSpawnX;
        }
        else
        {
            // Спавним справа, рыба плывет влево
            spawnX = rightSpawnX;
        }
        
        return new Vector3(spawnX, spawnY, 0);
    }

    // Методы для управления спавном во время игры
    public void SetSpawnInterval(float min, float max)
    {
        minSpawnInterval = min;
        maxSpawnInterval = max;
    }

    public void SetFishSpeed(float min, float max)
    {
        minFishSpeed = min;
        maxFishSpeed = max;
    }

    public void SetSpawnHeight(float min, float max)
    {
        minY = min;
        maxY = max;
    }

    // Визуализация границ спавна в редакторе
    private void OnDrawGizmosSelected()
    {
        if (mainCamera == null) return;

        Vector3 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        
        Gizmos.color = Color.red;
        
        // Левая граница спавна
        Vector3 leftSpawn = new Vector3(-screenBounds.x - spawnDistanceFromScreen, 0, 0);
        Gizmos.DrawLine(leftSpawn + Vector3.up * maxY, leftSpawn + Vector3.up * minY);
        
        // Правая граница спавна
        Vector3 rightSpawn = new Vector3(screenBounds.x + spawnDistanceFromScreen, 0, 0);
        Gizmos.DrawLine(rightSpawn + Vector3.up * maxY, rightSpawn + Vector3.up * minY);
        
        Gizmos.color = Color.yellow;
        
        // Границы экрана
        Gizmos.DrawLine(new Vector3(-screenBounds.x, maxY, 0), new Vector3(-screenBounds.x, minY, 0));
        Gizmos.DrawLine(new Vector3(screenBounds.x, maxY, 0), new Vector3(screenBounds.x, minY, 0));
    }
}
