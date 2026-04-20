using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;

    void Update()
    {
        
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Meteorite"))
        {
            Meteorite meteorite = collision.GetComponent<Meteorite>();
            if (meteorite != null)
            {
                meteorite.TakeDamage();
            }
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}