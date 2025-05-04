using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

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
        model.EnemyMoved += MoveEnemy;
        model.OnEnemyKilled += HandleEnemyKilled;
        model.OnGameOver += ShowGameOver;
        model.OnScoreChanged += UpdateScore;
        //OnMove + = HandleMove;
        controller = new GameController(this, model);
    }


    /*public event Action<float> OnMove;

    private void Update()
    {
        if (controller == null)
            return;

        controller.OnUpdate(Time.deltaTime);
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
            OnMove?.Invoke(horizontal);
    }

    private void HandleMove(float horizontal)
    {
        controller.OnPlayerInput(horizontal, Time.deltaTime);
    }*/



    private void Update()
    {
        if (controller == null)
            return;

        controller.OnUpdate(Time.deltaTime); // update de todos os objetos


        float horizontal = Input.GetAxisRaw("Horizontal");
        if(horizontal != 0)
            controller.OnPlayerInput(horizontal, Time.deltaTime);

        // Tiro: (Toma)
        bool shoot = Input.GetKeyDown(KeyCode.Space);
        if(shoot)
            controller.Shoot(shoot);

            
    }

    public void ShowMainMenu()
    {
        mainMenuPanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
        player?.SetActive(false);
    }

    public void StartGame()
    {
        //mainMenuPanel?.SetActive(false);
        //gameOverPanel?.SetActive(false);
        //player?.SetActive(true);
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
        const float tol = 0.05f;               // tolerância p/ float
        foreach (var kv in new Dictionary<Vector3, GameObject>(bullets))
        {
            if (Vector3.Distance(kv.Key, pos) < tol)
            {
                Destroy(kv.Value);             // mata o GO da bala
                bullets.Remove(kv.Key);        // tira do dicionário
                break;
            }
        }
    }

    public void SpawnEnemy(Vector3 pos)
    {
        var e = Instantiate(enemyPrefab, pos, Quaternion.identity);
        enemies[pos] = e;
    }

    public void MoveEnemy(Vector3 newPos, bool moveRight)
    {
        // Procura a instância cuja posição bate com o novo movimento,
        // seja deslocamento horizontal normal ou “drop” vertical.
        foreach (var kv in new Dictionary<Vector3, GameObject>(enemies))
        {
            Vector3 oldPos = kv.Key;
            float horizontalStep = Time.deltaTime * GameModel.enemySpeed;

            // 1) Movimento horizontal ─ X muda ±horizontalStep, Y mantém‑se
            bool horizontalMatch =
                Mathf.Approximately(oldPos.y, newPos.y) &&
                (
                    (moveRight  && Mathf.Approximately(oldPos.x + horizontalStep, newPos.x)) ||
                    (!moveRight && Mathf.Approximately(oldPos.x - horizontalStep, newPos.x))
                );

            // 2) Drop vertical ─ Y desce 1 unidade, X mantém‑se
            bool verticalMatch =
                Mathf.Approximately(oldPos.x, newPos.x) &&
                Mathf.Approximately(oldPos.y - 1f, newPos.y);

            if (horizontalMatch || verticalMatch)
            {
                GameObject go = kv.Value;
                enemies.Remove(oldPos);      // remove chave antiga
                go.transform.position = newPos;
                enemies[newPos] = go;        // adiciona nova chave
                break;
            }
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
    public void HandleEnemyKilled(Vector3 pos){
        ShowExplosion(pos);
        DestroyEnemy(pos);
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
