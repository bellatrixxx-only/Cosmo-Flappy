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

    private float gunTimer = 0f;
    private float gunDuration = 15;

    [Header("Интерфейс")]
    [SerializeField] private GameObject gunContainer;
    [SerializeField] private UnityEngine.UI.Slider gunBar;

    public Rigidbody2D rb;
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

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Flap();
        }

        if (hasWeapon)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0)
            {
                Shoot();
                fireTimer = fireRate;
            }
        }


        if (hasWeapon)
        {
            gunTimer -= Time.deltaTime;

           
            if (gunBar != null)
            {
                gunBar.value = gunTimer / gunDuration;
            }

            if (gunTimer <= 0)
            {
                DeactivateWeapon();
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
        gunTimer = gunDuration;

        if (gunContainer != null) gunContainer.SetActive(true);
        if (gunBar != null)
        {
            gunBar.value = 1f; 
        }

        CancelInvoke(nameof(DeactivateWeapon));
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