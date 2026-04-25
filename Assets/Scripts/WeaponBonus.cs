using UnityEngine;

public class BonusController : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 160f; 
    void Update()
    {
        
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

      
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }

    public void SetSpeed(float gameSpeed)
    {
        speed = gameSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ActivateWeapon();
            }

            Destroy(gameObject);
        }
    }
}