using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [Header("Настройки прочности")]
    [SerializeField] private int maxHealth = 2;
    private int currentHealth;

    private float speed = 4f; 

    void Start()
    {
        currentHealth = maxHealth;
    }
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        
        if (transform.position.x < -12)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage()
    {
        currentHealth--;
        Debug.Log($"Метеорит: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            DestroyMeteorite();
        }
    }

    void DestroyMeteorite()
    {
        
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Die();
            }
            Destroy(gameObject);
        }
    }
}