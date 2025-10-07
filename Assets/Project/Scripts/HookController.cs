using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookController : MonoBehaviour
{
    [Header("Ожидание")] [SerializeField] private float swingSpeed = 1.0f; // Скорость поворота в режиме ожидания
    [SerializeField] private float swingAngle = 45.0f; // Максимальный угол поворота (влево и вправо)

    [Header("Движение")] [SerializeField] private float castSpeed = 5.0f; // Скорость движения вперед
    [SerializeField] private float returnSpeed = 3.0f; // Скорость возврата
    [SerializeField] private float maxCastDistance = 10.0f; // Максимальное расстояние заброса


    [SerializeField] private List<Fish> fishPrefabs;

    [SerializeField] private GameObject infoObject;
    [SerializeField] private TextMeshProUGUI fichCostText;
    [SerializeField] private Image fishIcon;

    private bool isShowingInfo = false;

    private IEnumerator ShowFishInfo()
    {
        isShowingInfo = true;
        for (var index = 0; index < fishPrefabs.Count; index++)
        {
            var fish = fishPrefabs[0];
            infoObject.SetActive(true);
            fichCostText.text = fish.ScoreValue.ToString();
            fishIcon.sprite = fish.GetComponent<SpriteRenderer>().sprite;
            yield return new WaitForSeconds(1f);
            infoObject.SetActive(false);

            // Debug.Log(fishPrefabs);
            fishPrefabs.RemoveAt(0);
            Destroy(fish.gameObject);
            index--;

            yield return new WaitForSeconds(0.3f);
        }

        isShowingInfo = false;
    }

    // Перечисление состояний крючка
    public enum HookState
    {
        Waiting, // Ожидание (поворот влево-вправо)
        Casting, // Заброс (движение вперед)
        Returning // Возврат (движение назад)
    }

    // Текущее состояние крючка
    private HookState currentState = HookState.Waiting;

    // Сохраненные значения для движения
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 castDirection;
    private float currentDistance = 0f;

    // Флаг для определения направления поворота
    private bool isSwingingRight = true;

    private void Start()
    {
        // Сохраняем начальную позицию и поворот
        startPosition = transform.position;
        startRotation = transform.rotation;

        // Запускаем состояние ожидания
        SetState(HookState.Waiting);
    }

    private void Update()
    {
        switch (currentState)
        {
            case HookState.Waiting:
                SwingHook();
                break;

            case HookState.Casting:
                CastHook();
                break;

            case HookState.Returning:
                ReturnHook();
                break;
        }
    }

    // Поворот крючка влево-вправо в режиме ожидания
    private void SwingHook()
    {
        // Расчет угла поворота
        float targetAngle = isSwingingRight ? swingAngle : -swingAngle;

        // Плавный поворот с использованием Mathf.Sin
        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;
        transform.rotation = startRotation * Quaternion.Euler(0, 0, angle);
    }

    // Движение крючка вперед
    private void CastHook()
    {
        if (currentDistance < maxCastDistance)
        {
            // Двигаем крючок вперед
            float moveStep = castSpeed * Time.deltaTime;
            transform.position += transform.up * -1 * moveStep;
            Debug.DrawRay(transform.position, transform.up * -1, Color.red);
            currentDistance += moveStep;
        }
        else
        {
            // Когда достигли максимальной дистанции, переходим к возврату
            SetState(HookState.Returning);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
//        Debug.Log("Hook collided with: " + other.name);
        if (currentState == HookState.Waiting) return;
        var fish = other.GetComponent<Fish>();
        if (fish == null) return;
        fish.Stop();
        fishPrefabs.Add(fish);
        fish.transform.parent = transform;
    }

    // Возврат крючка в исходную позицию
    private void ReturnHook()
    {
        // Рассчитываем вектор направления к начальной точке
        Vector3 returnDirection = startPosition - transform.position;
        float distanceToStart = returnDirection.magnitude;

        if (distanceToStart > 0.1f)
        {
            // Двигаемся к начальной позиции
            returnDirection.Normalize();
            transform.position += returnDirection * returnSpeed * Time.deltaTime;
        }
        else
        {
            // Когда вернулись к начальной позиции, возвращаемся в режим ожидания
            transform.position = startPosition;
            transform.rotation = startRotation;
            currentDistance = 0f;
            SetState(HookState.Waiting);
            CatchFish();
        }
    }

    private void CatchFish()
    {
        var sum = 0;
        foreach (var fish in fishPrefabs)
        {
            if (fish != null)
            {
                fish.gameObject.SetActive(false);
                sum += fish.ScoreValue;
            }
            //  Destroy(fish.gameObject);
        }

        Wallet.Instance.AddCoins(sum);

        if (!isShowingInfo)
        {
            StartCoroutine(ShowFishInfo());
        }


        // fishPrefabs.Clear();
    }

    // Изменение состояния крючка
    public void SetState(HookState newState)
    {
        // Если изменение с ожидания на заброс, запоминаем текущее направление
        if (currentState == HookState.Waiting && newState == HookState.Casting)
        {
            castDirection = transform.forward;
        }

        currentState = newState;
    }

    // Публичный метод для заброса крючка
    public void CastHookNow()
    {
        if (currentState == HookState.Waiting)
        {
            SetState(HookState.Casting);
        }
    }

    // Получение текущего состояния крючка
    public HookState GetState()
    {
        return currentState;
    }
}