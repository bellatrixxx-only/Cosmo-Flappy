using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Настройки полета")]
    [SerializeField] private float flapForce = 5f;

    [Header("Стрельба")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.15f;

    [Header("Бонус - пушка")]
    [SerializeField] private bool hasWeapon = false;
    [SerializeField] private float weaponDuration = 15f;

    private Rigidbody2D rb;
    private bool isDead = false;
    private float fireTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false; 
        fireTimer = fireRate;
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameStarted) return;
        if (isDead) return;

        // Управление
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Flap();
        }

        // Стрельба
        if (hasWeapon)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0)
            {
                Shoot();
                fireTimer = fireRate;
            }
        }

        
        CheckScreenBoundaries();
    }

    private void CheckScreenBoundaries()
    {
        if (Camera.main == null) return;

       
        float camSize = Camera.main.orthographicSize;

        
        if (transform.position.y < -camSize - 1f)
        {
            Die();
        }

       
        if (transform.position.y > camSize)
        {
            Die();
        }
    }


    public void StartPlaying()
    {
        isDead = false;
        rb.simulated = true; 
    }

    void Flap()
    {
        rb.linearVelocity = new Vector2(0, flapForce);
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        }
    }

    public void ActivateWeapon()
    {
        hasWeapon = true;
        Invoke(nameof(DeactivateWeapon), weaponDuration);
    }

    private void DeactivateWeapon()
    {
        hasWeapon = false;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Meteorite") || collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }
}