using UnityEngine;

public class ObstaclePair : MonoBehaviour
{
    private float speed;
    private bool scoreCounted = false;
    private GameObject player;

    public void Init(float moveSpeed)
    {
        speed = moveSpeed;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -12)
        {
            Destroy(gameObject);
        }

        
        if (!scoreCounted && player != null)
        {
            float obstacleRightEdge = transform.position.x + 0.5f; 
            float playerLeftEdge = player.transform.position.x - 0.5f;

            if (obstacleRightEdge < playerLeftEdge)
            {
                scoreCounted = true;
                GameManager.Instance.AddScore();
            }
        }
    }
}