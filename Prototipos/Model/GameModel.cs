using UnityEngine;
using System.Linq;
using System.Collections.Generic;

// Corrige para usar os tipos do Unity
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public delegate void PositionChangedHandler(Vector3 newPosition);
public delegate void BulletFiredHandler(Vector3 bulletPosition);
public delegate void BulletMovedHandler(Vector3 newBulletPosition);
public delegate void BulletDestroyedHandler(Vector3 bulletPosition);
public delegate void EnemySpawnHandler(Vector3 enemyPosition);
public delegate void EnemyMovHandler(Vector3 newEnemyPosition, bool moveDir);
public delegate void EnemyKilledHandler(Vector3 enemyPosition);
public delegate void ScoreChangedHandler(int newScore);
public delegate void GameOverHandler();

public class GameModel
{
    // Eventos
    public event PositionChangedHandler OnPositionChanged;
    public event BulletFiredHandler BulletFired;
    public event BulletMovedHandler BulletMoved;
    public event BulletDestroyedHandler BulletDestroyed;
    public event EnemySpawnHandler OnEnemySpawn;
    public event EnemyMovHandler EnemyMoved;
    public event EnemyKilledHandler OnEnemyKilled;
    public event ScoreChangedHandler OnScoreChanged;
    public event GameOverHandler OnGameOver;

    // Estado do jogador
    private Vector3 playerPosition;

    // Estado de balas e inimigos
    private List<Vector3> bullets = new List<Vector3>();
    private List<Vector3> enemies = new List<Vector3>();

    // Outras variáveis
    public const float playerMovSpeed = 8.0f;
    public const float bulletSpeed = 10f;
    public const float enemySpeed = 5f;

    private bool enemyMoveRight = true;
    private readonly float maxLeft = -14f;
    private readonly float maxRight = 14f;
    private readonly float MaxY = 18f;
    private int score = 0;

    // Métodos

    public void StartNewGame()
    {
        int rows = 3; //3
        int columns = 5; //5
        float startX = -8f;
        float startY = 14f;
        float spacingX = 4f;
        float spacingY = 2f;

        bullets.Clear();
        enemies.Clear();
        score = 0;
        playerPosition = new Vector3(0f, -13.5f, 0f);
        Vector3 position = Vector3.zero;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float x = startX + (col * spacingX);
                float y = startY - (row * spacingY);
                float z = 0;
                Vector3 spawnPosition = new Vector3(x, y, z);

                EnemySpawn(spawnPosition);
            }
        }
        OnScoreChanged?.Invoke(score);
        OnPositionChanged?.Invoke(playerPosition);
    }

    public void OnUpdate(float deltaTime) // update a cada frame
    {
        UpdateBulletPos(deltaTime);


        UpdateEnemiesPos(deltaTime);
        DetectColision();
        //atualizar movimento das naves inimigas e dos tiros ativos
    }

    public void SetInitialPosition(Vector3 initialPosition)
    {
        playerPosition = initialPosition;
        OnPositionChanged?.Invoke(playerPosition);
    }

    public void Move(float direction, float deltaTime)
    {
        float deltaX = direction * playerMovSpeed * deltaTime;
        playerPosition.x = Mathf.Clamp(playerPosition.x + deltaX, maxLeft, maxRight);
        OnPositionChanged?.Invoke(playerPosition);
    }

    public void TryShot()
    {
        Vector3 bulletPos = playerPosition + new Vector3(0f, 1f, 0f);
        bullets.Add(bulletPos);
        BulletFired?.Invoke(bulletPos);
    }

    public void UpdateBulletPos(float deltaTime)
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            bullets[i] += Vector3.up * bulletSpeed * deltaTime;

            if (bullets[i].y > MaxY) // or same as enemy **
            {
                BulletDestroyed?.Invoke(bullets[i]);
                bullets.RemoveAt(i);
            }
            else
            {
                BulletMoved?.Invoke(bullets[i]);
            }
        }
    }

    public void EnemySpawn(Vector3 position)
    {
        enemies.Add(position);
        OnEnemySpawn?.Invoke(position);
    }

public void UpdateEnemiesPos(float deltaTime)
{
    float moveStep = enemySpeed * deltaTime;
    bool needDrop  = false;

    // 1) Detecta se ALGUM inimigo está na borda depois do próximo passo
    for (int i = 0; i < enemies.Count; i++)
    {
        float nextX = enemies[i].x + (enemyMoveRight ? moveStep : -moveStep);
        if (nextX >= maxRight || nextX <= maxLeft)
        {
            needDrop = true;
            enemyMoveRight = !enemyMoveRight; // inverte só UMA vez
            break;
        }
    }

    // 2) Se vai cruzar, desce TODOS e avisa a View
    if (needDrop)
    {
        for (int j = 0; j < enemies.Count; j++)
        {
            enemies[j] += Vector3.down;               // actualiza modelo
            EnemyMoved?.Invoke(enemies[j], enemyMoveRight); // <-- avisa drop
        }

        return; // não mover horizontalmente neste frame
    }

    // 3) Caso contrário, move horizontalmente
    Vector3 step = (enemyMoveRight ? Vector3.right : Vector3.left) * moveStep;

    for (int i = 0; i < enemies.Count; i++)
    {
        enemies[i] += step;
        EnemyMoved?.Invoke(enemies[i], enemyMoveRight);
    }
}
    /// <summary>
    /// Verifica colisões bala‑inimigo e actualiza listas / eventos.
    /// Considera hit se a distância em X e Y for < 0.5 unidades.
    /// </summary>
    private void DetectColision()
    {
        for (int b = bullets.Count - 1; b >= 0; b--)
        {
            Vector3 bulletPos = bullets[b];

            for (int e = enemies.Count - 1; e >= 0; e--)
            {
                Vector3 enemyPos = enemies[e];

                if (Mathf.Abs(bulletPos.x - enemyPos.x) < 0.5f &&
                    Mathf.Abs(bulletPos.y - enemyPos.y) < 0.5f)
                {
                    // Remove bala
                    bullets.RemoveAt(b);
                    BulletDestroyed?.Invoke(bulletPos);

                    // Regista hit no inimigo
                    EnemyHit(enemyPos);

                    break; // bala destruída, sai do loop interno
                }
            }
        }
    }
    public void EnemyHit(Vector3 position)
    {
        if (enemies.Remove(position))
        {
            score += 10;
            OnScoreChanged?.Invoke(score);
            OnEnemyKilled?.Invoke(position);
        }

        if (GameOverCheck())
        {
            OnGameOver?.Invoke();
        }
    }
    

    public bool GameOverCheck()
    {
        if (enemies.Count == 0)
        {
            return true;
        }

        foreach (var enemy in enemies)
        {
            if (enemy.y <= playerPosition.y)
            {
                return true;
            }
        }

        return false;
    }
}
