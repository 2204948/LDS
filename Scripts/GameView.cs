using UnityEngine;
using System.Collections.Generic;


public class GameView : MonoBehaviour, IGameView
{
    private GameController controller;
    private GameModel model;

    // Referências definidas no Inspector
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    // Mapas de posições para instâncias
    private List<GameObject> bullets = new List<GameObject>();
    private List<GameObject> enemies = new List<GameObject>();

    public event OnMoveHandler OnMove;
    public event OnShootHandler OnShoot;
    public event OnMainMenuStartHandler OnMainMenuStart;
    public event OnGameOverStartHandler OnGameOverStart;
    public event OnGameOverQuitHandler OnGameOverQuit;


    void Start()
    {
        model = new GameModel();
        model.OnPositionChanged += UpdatePlayerPosition;
        model.BulletFired += SpawnBullet;
        model.BulletMoved += MoveBullet;
        model.BulletDestroyed += DestroyBullet;
        model.OnEnemySpawn += SpawnEnemy;
        model.EnemyMoved += MoveEnemy;
        model.OnEnemyKilled += HandleEnemyKilled;
        model.OnGameOver += ShowGameOver;
        model.OnScoreChanged += UpdateScore;
        controller = new GameController(this, model);
    }

    private void Update()
    {
        if (controller == null)
            return;

        controller.OnUpdate(Time.deltaTime);

        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
            OnMove?.Invoke(horizontal, Time.deltaTime);
        bool shoot = Input.GetKeyDown(KeyCode.Space);
        if (shoot)
            OnShoot?.Invoke();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
        player?.SetActive(false);
    }

    public void StartGame()
    {
        mainMenuPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        player?.SetActive(true);
    }

    public void ShowGameOver()
    {
        mainMenuPanel?.SetActive(false);
        gameOverPanel?.SetActive(true);
        player?.SetActive(false);
    }

    public void OnMainMenuStartButton()
    {
        OnMainMenuStart?.Invoke();
    }

    public void OnGameOverStartButton()
    {
        OnGameOverStart?.Invoke();
    }

    public void OnGameOverQuitButton()
    {
        OnGameOverQuit?.Invoke();
    }

    //  Métodos de atualização dos objetos

    private void UpdatePlayerPosition(Coord pos)
    {
        Vector3 vectorPos = CoordToVector(pos);

        if (player != null)
            player.transform.position = vectorPos;
    }

    private void SpawnBullet(Coord pos)
    {
        Vector3 vectorPos = CoordToVector(pos);

        var b = Instantiate(bulletPrefab, vectorPos, Quaternion.identity);
        bullets.Add(b);
    }

    public void MoveBullet(List<Coord> newPos)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            Vector3 vectorPos = CoordToVector(newPos[i]);
            bullets[i].transform.position = vectorPos;
        }
    }

    public void DestroyBullet(int bullet)
    {
        Destroy(bullets[bullet]);
        bullets.RemoveAt(bullet);
    }

    public void SpawnEnemy(List<Coord> enemyPosition)
    {
        foreach (Coord enemy in enemyPosition)
        {
            Vector3 vectorPos = CoordToVector(enemy);
            
            var e = Instantiate(enemyPrefab, vectorPos, Quaternion.identity);
            enemies.Add(e);
        }
    }

    public void MoveEnemy(List<Coord> newPos)
    {

        for (int i = 0; i < enemies.Count; i++)
        {
            Vector3 vectorPos = CoordToVector(newPos[i]);

            enemies[i].transform.position = vectorPos;
        }
    }

    public void ShowExplosion(Vector3 pos)
    {
        var ex = Instantiate(explosionPrefab, pos, Quaternion.identity);
        Destroy(ex, .1f); // .1 para ser apenas um flash
    }

    public void HandleEnemyKilled(int enemy)
    {
        Vector3 vectorPos = enemies[enemy].transform.position;

        ShowExplosion(vectorPos);
        Destroy(enemies[enemy]);
        enemies.RemoveAt(enemy);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    // Exemplo de feedback visual ao dano (opcional)
    public void FlashPlayerDamage()
    {
        StartCoroutine(FlashRed());
    }

    private System.Collections.IEnumerator FlashRed()
    {
        var sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            var orig = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            sr.color = orig;
        }
    }

    private Vector3 CoordToVector(Coord coord)
    {
        return new Vector3(coord.x, coord.y, coord.z);
    }
}
