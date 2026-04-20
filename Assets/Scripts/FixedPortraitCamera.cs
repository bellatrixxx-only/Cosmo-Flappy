using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedPortraitCamera : MonoBehaviour
{
    [Header("Target Aspect (Width / Height)")]
    [SerializeField] private float targetAspect = 0.5625f; // 9:16 = 0.5625

    private Camera _camera;
    private Rect _defaultRect;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        _defaultRect = _camera.rect;
    }

    void Start()
    {
        ApplyFixedAspect();
    }

    void Update()
    {
        
#if UNITY_EDITOR
        ApplyFixedAspect();
#endif
    }

    void ApplyFixedAspect()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;

        if (windowAspect > targetAspect)
        {
            
            float viewportWidth = targetAspect / windowAspect;
            float xOffset = (1f - viewportWidth) / 2f;

            GetComponent<Camera>().rect = new Rect(xOffset, 0f, viewportWidth, 1f);
        }
        else
        {
            _camera.rect = _defaultRect;
        }
    }
}