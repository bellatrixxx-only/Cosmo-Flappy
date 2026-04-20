using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Настройки движения")]
    [Tooltip("Базовая скорость движения слоя (пиксели в секунду). Отрицательное значение = движение влево.")]
    public float scrollSpeed = -2f;

    [Tooltip("Множитель скорости относительно общей скорости игры. 1.0 = стандарт, 0.5 = медленно (дальний план).")]
    public float speedMultiplier = 1f;

    private float spriteWidth;
    private Vector3 startPosition;
    private GameManager gameManager;
    private float baseGameSpeed = 3f;

    void Start()
    {
        startPosition = transform.position;
        gameManager = GameManager.Instance;

        
        if (gameManager != null)
        {
          
            baseGameSpeed = 3f; 
        }

       
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length > 0 && renderers[0].sprite != null)
        {
            spriteWidth = renderers[0].sprite.bounds.size.x;
        }
        else
        {
            Debug.LogWarning("[ParallaxLayer] Спрайт не найден. Бесшовный цикл может работать некорректно.");
        }
    }

    void Update()
    {
        float finalSpeed = scrollSpeed * speedMultiplier;

        if (gameManager != null && gameManager.IsGameStarted)
        {
            finalSpeed *= (baseGameSpeed / 3f);
        }
        transform.Translate(Vector3.right * finalSpeed * Time.deltaTime);
              if (transform.position.x < startPosition.x - spriteWidth)
        {
            transform.position = new Vector3(transform.position.x + spriteWidth, startPosition.y, startPosition.z);
        }
    }
}