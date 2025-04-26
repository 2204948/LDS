using UnityEngine;
using System.Collections.Generic;

public class GameView : MonoBehaviour
{
    private GameController controller;
    private GameModel model;

    // Referências definidas no Inspector
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    // Mapas de posições para instâncias
    private Dictionary<Vector3, GameObject> bullets = new();
    private Dictionary<Vector3, GameObject> enemies = new();

    void Start()
    {
        model = new GameModel();
        model.OnPositionChanged += UpdatePlayerPosition;
        model.BulletFired += SpawnBullet;
        model.BulletMoved += MoveBullet;
        model.BulletDestroyed += DestroyBullet;
        model.OnEnemySpawn += SpawnEnemy;
        model.OnEnemyKilled += ShowExplosion;
        model.OnGameOver += ShowGameOver;
        model.OnScoreChanged += UpdateScore;
        controller = new GameController(this, model);
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if(horizontal != 0)
            controller.OnPlayerInput(horizontal, Time.deltaTime);
        bool shoot = Input.GetKeyDown(KeyCode.Space);
        if(shoot)
            controller.Shoot(shoot, Time.deltaTime);
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
        gameOverPanel?.SetActive(true);
    }

    public void OnMainMenuStartButton()
    {
        controller.StartGame();
    }

    public void OnGameOverStartButton()
    {
        controller.StartGame();
    }

    public void OnGameOverQuitButton()
    {
        controller.ShowMenu();
    }

    //  Métodos de atualização dos objetos

    private void UpdatePlayerPosition(Vector3 pos)
    {
        if (player != null)
            player.transform.position = pos;
    }

    private void SpawnBullet(Vector3 pos)
    {
        var b = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bullets[pos] = b;
    }

    public void MoveBullet(Vector3 newPos)
    {
        // encontra a entrada cujo key é a posição anterior mais próxima
        foreach (var kv in new Dictionary<Vector3, GameObject>(bullets))
        {
            if (kv.Key.x == newPos.x && Mathf.Approximately(kv.Key.y + Time.deltaTime * GameModel.bulletSpeed, newPos.y))
            {
                var go = kv.Value;
                bullets.Remove(kv.Key);
                go.transform.position = newPos;
                bullets[newPos] = go;
                break;
            }
        }
    }

    public void DestroyBullet(Vector3 pos)
    {
        if (bullets.TryGetValue(pos, out var b))
        {
            Destroy(b);
            bullets.Remove(pos);
        }
    }

    public void SpawnEnemy(Vector3 pos)
    {
        var e = Instantiate(enemyPrefab, pos, Quaternion.identity);
        enemies[pos] = e;
    }

    public void ShowExplosion(Vector3 pos)
    {
        var ex = Instantiate(explosionPrefab, pos, Quaternion.identity);
        Destroy(ex, 1f);
    }

    public void DestroyEnemy(Vector3 pos)
    {
        if (enemies.TryGetValue(pos, out var e))
        {
            Destroy(e);
            enemies.Remove(pos);
        }
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
}
