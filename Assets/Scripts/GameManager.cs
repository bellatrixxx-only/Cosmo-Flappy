using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private Camera mainCam;

  

    [Header("Префабы")]
    [SerializeField] private GameObject obstaclePairPrefab;
    [SerializeField] private GameObject meteoritePrefab;
    [SerializeField] private GameObject bonusPrefab;

    [Header("Спавн препятствий")]
    [SerializeField] private float startObstacleInterval = 2.5f; 
    [SerializeField] private float minObstacleInterval = 0.7f;  
    [SerializeField] private float obstacleIntervalStep = 0.1f; 
    [SerializeField] private float gapSize = 2.5f;
    [SerializeField] private float spawnOffsetX = -1.5f;

    [Header("Метеориты (после 10 очков)")]
    [SerializeField] private float startMeteoriteInterval = 4f;
    [SerializeField] private float minMeteoriteInterval = 1.2f;
    [SerializeField] private float meteoriteIntervalStep = 0.3f;

    [Header("Бонусы (после 10 очков)")]
    [SerializeField] private float startBonusInterval = 15f;
    [SerializeField] private float maxBonusInterval = 40f;
    [SerializeField] private float bonusIntervalStep = 2f;

    [Header("Скорость игры")]
    [SerializeField] private float startGameSpeed = 3f;
    [SerializeField] private float speedStep = 0.4f;

    [Header("Фон")]
    [SerializeField] private List<ParallaxLayer> parallaxLayers;
    [SerializeField] private float bgSpeedRatio = 0.5f;

    [Header("Интерфейс")]
    [SerializeField] private GameObject hud;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

  
    private float score = 0;
    private bool isGameActive = false;
    private bool isGameStarted = false;

    private float currentObstacleInterval;
    private float currentMeteoriteInterval;
    private float currentBonusInterval;
    private float currentGameSpeed;

    private float obstacleTimer;
    private float meteoriteTimer;
    private float bonusTimer;


    public bool IsGameStarted => isGameStarted;

    void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null) Debug.LogError("❌ Main Camera not found!");

        if (hud != null) hud.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (startScreen != null) startScreen.SetActive(true);

        ResetGameValues();
    }

    private void ResetGameValues()
    {
        currentGameSpeed = startGameSpeed;
        currentObstacleInterval = startObstacleInterval;
        currentMeteoriteInterval = startMeteoriteInterval;
        currentBonusInterval = startBonusInterval;

        obstacleTimer = currentObstacleInterval;
        meteoriteTimer = currentMeteoriteInterval;
        bonusTimer = currentBonusInterval;

        UpdateBackgroundSpeed();
    }

    void Update()
    {
        if (!isGameStarted || !isGameActive) return;

        SpawnObstacle();
        SpawnMeteorite();
        SpawnBonus();
    }

    private float GetRightScreenEdge()
    {
        return mainCam.orthographicSize * mainCam.aspect;
    }

    private Vector2 GetSpawnPosition()
    {
        float spawnX = GetRightScreenEdge() + spawnOffsetX;
        float minY = -mainCam.orthographicSize + 1.5f;
        float maxY = mainCam.orthographicSize - 1.5f;
        return new Vector2(spawnX, Random.Range(minY, maxY));
    }

    private Vector2 GetSafeSpawnPosition(float minY, float maxY)
    {
        int attempts = 0;
        int maxAttempts = 30;
        Vector2 spawnPos;
        LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");

       
        GameObject[] pipes = GameObject.FindGameObjectsWithTag("Pipe");

        do
        {
            float spawnX = GetRightScreenEdge() + spawnOffsetX;
            float spawnY = Random.Range(minY, maxY);
            spawnPos = new Vector2(spawnX, spawnY);

           
            if (Physics2D.OverlapCircle(spawnPos, 0.6f, obstacleLayer) != null)
            {
                attempts++;
                continue;
            }

            bool isInsideGap = false;
            float safetyMargin = 0.5f;

            foreach (var pipeObj in pipes)
            {
                if (pipeObj.transform.position.x > -5f) 
                {
                    Collider2D topCol = null;
                    Collider2D botCol = null;

                    foreach (Transform child in pipeObj.transform)
                    {
                        if (child.name.Contains("TopObstacle"))
                            topCol = child.GetComponent<Collider2D>();
                        else if (child.name.Contains("BottomObstacle"))
                            botCol = child.GetComponent<Collider2D>();
                    }

                    if (topCol != null && botCol != null)
                    {
                        float gapTopY = topCol.bounds.min.y;
                        float gapBotY = botCol.bounds.max.y; 

                        if (spawnY > gapBotY - safetyMargin && spawnY < gapTopY + safetyMargin)
                        {
                            isInsideGap = true;
                            break;
                        }
                    }
                }
            }

            if (!isInsideGap) return spawnPos;
            attempts++;
        } while (attempts < maxAttempts);

        return new Vector2(GetRightScreenEdge() + spawnOffsetX, Random.Range(minY, maxY));
    }

    void SpawnObstacle()
    {
        obstacleTimer -= Time.deltaTime;
        if (obstacleTimer <= 0)
        {
            Vector2 pos = GetSpawnPosition();
            GameObject pair = Instantiate(obstaclePairPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);

            foreach (Transform child in pair.GetComponentsInChildren<Transform>())
            {
                if (child.name.Contains("TopObstacle"))
                    child.localPosition = new Vector3(0, gapSize / 2 + 2.5f, 0);
                else if (child.name.Contains("BottomObstacle"))
                    child.localPosition = new Vector3(0, -gapSize / 2 - 2.5f, 0);
            }

            var movement = pair.GetComponent<ObstaclePair>();
            if (movement == null) movement = pair.AddComponent<ObstaclePair>();
            movement.Init(currentGameSpeed);

            obstacleTimer = currentObstacleInterval;
        }
    }

    void SpawnMeteorite()
    {
        if (score < 5) return;

        meteoriteTimer -= Time.deltaTime;
        if (meteoriteTimer <= 0)
        {
            Vector2 pos = GetSafeSpawnPosition(-mainCam.orthographicSize + 1f, mainCam.orthographicSize - 1f);
            GameObject meteorite = Instantiate(meteoritePrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            meteorite.GetComponent<Meteorite>().SetSpeed(currentGameSpeed);

            meteoriteTimer = currentMeteoriteInterval;
        }
    }

    void SpawnBonus()
    {
        if (score < 5) return;

        bonusTimer -= Time.deltaTime;
        if (bonusTimer <= 0)
        {
            Vector2 pos = GetSafeSpawnPosition(-mainCam.orthographicSize + 1f, mainCam.orthographicSize - 1f);
            GameObject bonus = Instantiate(bonusPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            bonus.GetComponent<BonusController>().SetSpeed(currentGameSpeed);

            bonusTimer = currentBonusInterval;
        }
    }

    public void StartTheGame()
    {
        isGameStarted = true;
        isGameActive = true;


        AudioManager.Instance.PlayMusic();

        if (startScreen != null) startScreen.SetActive(false);
        if (hud != null) hud.SetActive(true);

        FindObjectOfType<PlayerController>()?.StartPlaying();
    }

    public void AddScore()
    {
        if (!isGameActive) return;

        score++;
        if (scoreText != null) scoreText.text = ((int)score).ToString("000000");

        if ((int)score % 10 == 0 && score >= 10)
        {
            IncreaseDifficulty();
        }
    }

    void IncreaseDifficulty()
    {
        currentGameSpeed += speedStep;
        currentObstacleInterval = Mathf.Max(minObstacleInterval, currentObstacleInterval - obstacleIntervalStep);
        currentMeteoriteInterval = Mathf.Max(minMeteoriteInterval, currentMeteoriteInterval - meteoriteIntervalStep);
        currentBonusInterval = Mathf.Min(maxBonusInterval, currentBonusInterval + bonusIntervalStep);

        UpdateBackgroundSpeed();
    }

    void UpdateBackgroundSpeed()
    {
        float bgSpeed = -currentGameSpeed * bgSpeedRatio;
        foreach (var layer in parallaxLayers)
        {
            layer?.SetSpeed(bgSpeed);
        }
    }

    public void GameOver()
    {
        if (!isGameActive) return;
        isGameActive = false;

        AudioManager.Instance.StopMusic();

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            if (player.rb != null) player.rb.linearVelocity = Vector2.zero;
            Destroy(player.gameObject);
        }

        foreach (var bg in FindObjectsOfType<ParallaxLayer>())
        {
            bg.enabled = false;
        }

        int currentScore = (int)score;
        int bestScore = Mathf.Max(currentScore, PlayerPrefs.GetInt("HighScore", 0));

        if (currentScore >= bestScore)
        {
            PlayerPrefs.SetInt("HighScore", bestScore);
            PlayerPrefs.Save();
        }

        if (finalScoreText != null) finalScoreText.text = $"Your Score: {currentScore:000000}";
        if (highScoreText != null) highScoreText.text = $"Your Best: {bestScore:000000}";

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (hud != null) hud.SetActive(false);
        enabled = false;
    }

    public void RestartGame()
    {
        AudioManager.Instance.RestartMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}