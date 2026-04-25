using UnityEngine;
using System.Collections.Generic;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = -1.5f; 
    private List<Transform> segments = new List<Transform>();
    private float segmentWidth;
    private float leftEdgeLimit;
    private bool isInitialized = false;

    void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        if (transform.childCount == 0) return;
        Transform template = transform.GetChild(0);
        SpriteRenderer sr = template.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        segmentWidth = sr.bounds.size.x;

        if (Camera.main != null)
        {
            float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            leftEdgeLimit = -camHalfWidth - segmentWidth;
        }

        float screenFullWidth = Camera.main ? Camera.main.orthographicSize * Camera.main.aspect * 2f : 20f;
        int neededSegments = Mathf.CeilToInt(screenFullWidth / segmentWidth) + 2;

        for (int i = 0; i < neededSegments; i++)
        {
            Transform segment;
            if (i == 0)
            {
                segment = template;
            }
            else
            {
                segment = Instantiate(template, transform);
            }
            Vector3 pos = segment.position;
            pos.x = i * segmentWidth;
            segment.position = pos;
            segments.Add(segment);
        }
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        float moveStep = scrollSpeed * Time.deltaTime;
        foreach (var seg in segments)
        {
            seg.Translate(Vector3.right * moveStep, Space.World);
        }

        foreach (var seg in segments)
        {
            if (seg.position.x + segmentWidth / 2f < leftEdgeLimit)
            {
                float rightmostX = float.MinValue;
                foreach (var other in segments)
                {
                    if (other.position.x > rightmostX)
                        rightmostX = other.position.x;
                }
                Vector3 newPos = seg.position;
                newPos.x = rightmostX + segmentWidth;
                seg.position = newPos;
            }
        }
    }

    public void SetSpeed(float speed)
    {
        scrollSpeed = speed;
    }
}