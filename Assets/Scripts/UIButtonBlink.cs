using UnityEngine;
using TMPro;

public class UIButtonBlink : MonoBehaviour
{
    [Header("Настройки мигания")]
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;

    private TextMeshProUGUI textComponent;
    private Color originalColor;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            originalColor = textComponent.color;
        }
    }

    void Update()
    {
        if (textComponent == null) return;

        
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, maxAlpha - minAlpha) + minAlpha;
        textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }
}