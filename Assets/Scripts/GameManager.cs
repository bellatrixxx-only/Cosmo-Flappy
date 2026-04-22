using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private Camera mainCam;

    [Header("Префабы")]
    [SerializeField] private GameObject obstaclePairPrefab;
    [SerializeField] private GameObject meteoritePrefab;
    [SerializeField] private GameObject bonusPrefab;

    [Header("Настройки спавна")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float gapSize = 2.5f;
    [SerializeField] private float spawnOffsetX = 1.5f;

    [Header("Метеориты")]
    [SerializeField] private float meteoriteSpawnInterval = 3f;
    [SerializeField] private float baseMeteoriteInterval = 3f;
    [SerializeField] private float intervalDecrease = 0.2f;
    [SerializeField] private float minMeteoriteInterval = 1f;
    private float currentMeteoriteInterval;

    [Header("Бонусы")]
    [SerializeField] private float bonusSpawnInterval = 12f;
    private float currentBonusInterval;

    [Header("Сложность")]
    [SerializeField] private float baseGameSpeed = 3f;
    [SerializeField] private float speedIncrement = 0.5f;
    private float currentGameSpeed;

    [Header("Интерфейс")]
    [SerializeField] private GameObject hud;                
    [SerializeField] private TextMeshProUGUI scoreText;     
    [SerializeField] private GameObject startScreen;        

    [Header("Game Over Screen")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText; 

    private float score = 0;
    private bool isGameActive = false;
    private bool isGameStarted = false;

    private float spawnTimer;
    private float meteoriteTimer;
    private float bonusTimer;

    public bool IsGameStarted => isGameStarted;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null) Debug.LogError("Main Camera not found!");

        
        if (hud != null) hud.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (startScreen != null) startScreen.SetActive(true);

        currentGameSpeed = baseGameSpeed;
        currentMeteoriteInterval = baseMeteoriteInterval;
        currentBonusInterval = bonusSpawnInterval;

        spawnTimer = spawnInterval;
        meteoriteTimer = meteoriteSpawnInterval;
        bonusTimer = bonusSpawnInterval;
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
        float rightEdge = GetRightScreenEdge();
        float spawnX = rightEdge + spawnOffsetX;
        float minY = -mainCam.orthographicSize + 1.5f;
        float maxY = mainCam.orthographicSize - 1.5f;
        float spawnY = Random.Range(minY, maxY);
        return new Vector2(spawnX, spawnY);
    }

    private Vector2 GetSafeSpawnPosition(float minY, float maxY)
    {
        int attempts = 0;
        int maxAttempts = 10;
        Vector2 spawnPos;
        LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");

        do
        {
            float spawnX = GetRightScreenEdge() + spawnOffsetX;
            float spawnY = Random.Range(minY, maxY);
            spawnPos = new Vector2(spawnX, spawnY);
            Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.5f, obstacleLayer);
            if (hit == null) return spawnPos;
            attempts++;
        } while (attempts < maxAttempts);

        return new Vector2(GetRightScreenEdge() + spawnOffsetX, Random.Range(minY, maxY));
    }

    void SpawnObstacle()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            Vector2 pos = GetSpawnPosition();
            GameObject pair = Instantiate(obstaclePairPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);

            Transform[] children = pair.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.name.Contains("TopObstacle"))
                    child.localPosition = new Vector3(0, gapSize / 2 + 2.5f, 0);
                else if (child.name.Contains("BottomObstacle"))
                    child.localPosition = new Vector3(0, -gapSize / 2 - 2.5f, 0);
            }

            var movement = pair.GetComponent<ObstaclePair>();
            if (movement == null) movement = pair.AddComponent<ObstaclePair>();
            movement.Init(currentGameSpeed);

            spawnTimer = spawnInterval;
        }
    }

    void SpawnMeteorite()
    {
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

        // Логика UI при старте
        if (startScreen != null) startScreen.SetActive(false); 
        if (hud != null) hud.SetActive(true);                  

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) player.StartPlaying();
    }

    public void AddScore()
    {
        if (!isGameActive) return;
        score++;

      
        if (scoreText != null)
        {
            scoreText.text = ((int)score).ToString("000000");
        }

        if ((int)score % 10 == 0 && score > 0) IncreaseDifficulty();
    }

    void IncreaseDifficulty()
    {
        currentGameSpeed += speedIncrement;
        currentMeteoriteInterval = Mathf.Max(minMeteoriteInterval, currentMeteoriteInterval - intervalDecrease);
    }

    public void GameOver()
    {
        if (!isGameActive) return;
        isGameActive = false;

        int currentScore = (int)score;
        int bestScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("HighScore", bestScore);
            PlayerPrefs.Save();
        }

       
        if (finalScoreText != null) finalScoreText.text = currentScore.ToString("000000");
        if (highScoreText != null)
        {
            highScoreText.text = $"Best: {bestScore:000000}";
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(true); 
        if (hud != null) hud.SetActive(false);                   
    }

    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}