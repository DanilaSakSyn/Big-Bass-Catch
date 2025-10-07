using UnityEngine;

public class Fish : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool movingRight = true;
    [SerializeField] private bool isMove = true;
    
    private Camera mainCamera;
    private float screenBounds;
    private bool isInitialized = false;

    [SerializeField] private int scoreValue = 1;
    public int ScoreValue => scoreValue;
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Рассчитываем границы экрана с небольшим запасом
            screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2f;
            isInitialized = true;
        }
    }

    private void Update()
    {
        if (!isInitialized) return;

        // Движение рыбы
        MoveFish();
        
        // Проверяем, вышла ли рыба за границы экрана
        CheckBounds();
    }

    private void MoveFish()
    {
        if (!isMove)return;
        // Двигаем рыбу в зависимости от направления
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void CheckBounds()
    {
        // Если рыба ушла за пределы экрана, уничтожаем её
        if (movingRight && transform.position.x > screenBounds)
        {
            DestroyFish(); 
        }
        else if (!movingRight && transform.position.x < -screenBounds)
        {
            DestroyFish();
        }
    }

    public void SetDirection(bool moveRight)
    {
        movingRight = moveRight;
        
        // Поворачиваем спрайт в зависимости от направления
        if (!moveRight)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void DestroyFish()
    {
        Destroy(gameObject);
    }

    public void Stop()
    {
        isMove = false;
    }
}
