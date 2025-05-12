using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;


public class GameView : MonoBehaviour, IGameView
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
        bullets[vectorPos] = b;
    }

    public void MoveBullet(Coord newPos)
    {
        Vector3 vectorPos = CoordToVector(newPos);

        // encontra a entrada cujo key é a posição anterior mais próxima
        foreach (var kv in new Dictionary<Vector3, GameObject>(bullets))
        {
            if (kv.Key.x == vectorPos.x && Mathf.Approximately(kv.Key.y + Time.deltaTime * GameModel.bulletSpeed, vectorPos.y))
            {
                var go = kv.Value;
                bullets.Remove(kv.Key);
                go.transform.position = vectorPos;
                bullets[vectorPos] = go;
                break;
            }
        }
    }

    public void DestroyBullet(Coord pos)
    {
        Vector3 vectorPos = CoordToVector(pos);

        const float tol = 0.05f;               // tolerância p/ float
        foreach (var kv in new Dictionary<Vector3, GameObject>(bullets))
        {
            if (Vector3.Distance(kv.Key, vectorPos) < tol)
            {
                Destroy(kv.Value);             // mata o GO da bala
                bullets.Remove(kv.Key);        // tira do dicionário
                break;
            }
        }
    }

    public void SpawnEnemy(Coord pos)
    {
        Vector3 vectorPos = new Vector3(pos.x, pos.y, pos.z);

        var e = Instantiate(enemyPrefab, vectorPos, Quaternion.identity);
        enemies[vectorPos] = e;
    }

    public void MoveEnemy(Coord newPos, bool moveRight)
    {
        Vector3 vectorPos = CoordToVector(newPos);

        // Procura a instância cuja posição bate com o novo movimento,
        // seja deslocamento horizontal normal ou “drop” vertical.
        foreach (var kv in new Dictionary<Vector3, GameObject>(enemies))
        {
            Vector3 oldPos = kv.Key;
            float horizontalStep = Time.deltaTime * GameModel.enemySpeed;

            // 1) Movimento horizontal ─ X muda ±horizontalStep, Y mantém‑se
            bool horizontalMatch =
                Mathf.Approximately(oldPos.y, vectorPos.y) &&
                (
                    (moveRight && Mathf.Approximately(oldPos.x + horizontalStep, vectorPos.x)) ||
                    (!moveRight && Mathf.Approximately(oldPos.x - horizontalStep, vectorPos.x))
                );

            // 2) Drop vertical ─ Y desce 1 unidade, X mantém‑se
            bool verticalMatch =
                Mathf.Approximately(oldPos.x, vectorPos.x) &&
                Mathf.Approximately(oldPos.y - 1f, vectorPos.y);

            if (horizontalMatch || verticalMatch)
            {
                GameObject go = kv.Value;
                enemies.Remove(oldPos);      // remove chave antiga
                go.transform.position = vectorPos;
                enemies[vectorPos] = go;        // adiciona nova chave
                break;
            }

            /*for(i; 0->nrenimigos)
                enemies[i].transform.position = vectorPos*/
        }
    }

    public void ShowExplosion(Vector3 pos)
    {
        var ex = Instantiate(explosionPrefab, pos, Quaternion.identity);
        Destroy(ex, .1f); // .1 para ser apenas um flash
    }


    public void DestroyEnemy(Vector3 pos)
    {
        // pequena tolerância, para evitar falhas por arredondamento de floats
        const float tol = 0.05f;

        // percorre uma cópia para evitar modificação durante iteração
        foreach (var kv in new Dictionary<Vector3, GameObject>(enemies))
        {
            if (Vector3.Distance(kv.Key, pos) < tol)
            {
                Destroy(kv.Value);      // destrói o GameObject
                enemies.Remove(kv.Key); // remove a entrada do dicionário
                break;
            }
        }
    }
    public void HandleEnemyKilled(Coord pos)
    {
        Vector3 vectorPos = CoordToVector(pos);
        
        ShowExplosion(vectorPos);
        DestroyEnemy(vectorPos);
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
