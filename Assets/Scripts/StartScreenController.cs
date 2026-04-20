using UnityEngine;
using TMPro;

public class StartScreenController : MonoBehaviour
{
    [Header("UI Ссылки")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private TextMeshProUGUI recordText;
    [SerializeField] private TextMeshProUGUI startHintText;

    [Header("Настройки")]
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField] private GameManager gameManager;

    private bool isHintVisible = true;
    private Color originalHintColor;

    void Start()
    {
        originalHintColor = startHintText.color;

      
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScore > 0)
        {
            recordText.text = $"Your record {highScore}";
            recordText.gameObject.SetActive(true);
        }
        else
        {
            recordText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
        float alpha = Mathf.Sin(Time.time * blinkSpeed);
        startHintText.color = new Color(originalHintColor.r, originalHintColor.g, originalHintColor.b, Mathf.Abs(alpha));

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        if (gameManager != null)
        {
            gameManager.StartTheGame();
            gameObject.SetActive(false); 
        }
    }
}