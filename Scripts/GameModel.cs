using System;
using System.Collections.Generic;


public class GameModel : IGameModel
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
    public event EnemyBulletFiredHandler EnemyBulletFired;
    public event EnemyBulletMovedHandler EnemyBulletMoved;
    public event EnemyBulletDestroyedHandler EnemyBulletDestroyed;
    // Estado do jogador
    private Coord playerPosition;

    // Estado de balas e inimigos
    private List<Coord> bullets = new List<Coord>();
    private List<Coord> enemies = new List<Coord>();
    private Coord? enemyBullet;

    // Outras variáveis
    private const float playerMovSpeed = 8.0f;
    private const float bulletSpeed = 10f;
    private float enemySpeed = 5f;

    private bool enemyMoveRight = true;
    private readonly float maxLeft = -14f;
    private readonly float maxRight = 14f;
    private readonly float MaxY = 18f;
    private readonly float minY = -14f;
    private int score = 0;

    // Métodos

    public void StartNewGame()
    {
        bullets.Clear();
        enemies.Clear();
        score = 0;
        enemyBullet = null;
        playerPosition = new Coord(0f, -13.5f, 0f);
        SpawnEnemies();

        OnScoreChanged?.Invoke(score);
        OnPositionChanged?.Invoke(playerPosition);
    }

    private void SpawnEnemies()
    {
        int rows = 3; //3
        int columns = 5; //5
        float startX = -8f;
        float startY = 14f;
        float spacingX = 4f;
        float spacingY = 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float x = startX + (col * spacingX);
                float y = startY - (row * spacingY);
                float z = 0;
                Coord spawnPosition = new Coord(x, y, z);

                enemies.Add(spawnPosition);
            }
        }
        OnEnemySpawn?.Invoke(enemies);
    }

    public void OnUpdate(float deltaTime) // update a cada frame
    {
        UpdateBulletPos(deltaTime);
        UpdateEnemiesPos(deltaTime);
        DetectColision();
        UpdateEnemyBulletPos(deltaTime);
        if (enemyBullet == null)
            enemyShot();
        DetectEnemyBulletColision();
        DetectEnemyColision();
        //atualizar movimento das naves inimigas e dos tiros ativos
    }

    public void Move(float direction, float deltaTime)
    {
        float deltaX = direction * playerMovSpeed * deltaTime;
        playerPosition.x = Coord.Clamp(playerPosition.x + deltaX, maxLeft, maxRight);
        OnPositionChanged?.Invoke(playerPosition);
    }

    public void enemyShot()
    {
        if (enemies.Count > 0)
        {
            Random rnd = new Random();
            int enemy = rnd.Next(0, enemies.Count);

            Coord enemyPosition = new Coord(enemies[enemy].x, enemies[enemy].y, enemies[enemy].z);
            Coord bulletPos = enemies[enemy] + new Coord(0, -1, 0);
            enemyBullet = bulletPos;
            EnemyBulletFired?.Invoke(bulletPos);
        }
    }

    public void TryShot()
    {
        Coord playerPositionC = new Coord(playerPosition.x, playerPosition.y, playerPosition.z);
        Coord bulletPos = playerPositionC + new Coord(0, 1, 0);
        bullets.Add(bulletPos);
        BulletFired?.Invoke(bulletPos);
    }

    private void UpdateBulletPos(float deltaTime)
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            bullets[i] += Coord.Up(bulletSpeed * deltaTime);

            if (bullets[i].y > MaxY) // or same as enemy **
            {
                bullets.RemoveAt(i);
                BulletDestroyed?.Invoke(i);
            }
        }
        BulletMoved?.Invoke(bullets);
    }

    private void UpdateEnemyBulletPos(float deltaTime)
    {
        if (enemyBullet.HasValue)
        {
            Coord bullet = enemyBullet.Value;
            bullet += Coord.Down(bulletSpeed * deltaTime);
            if (bullet.y < minY) // or same as enemy **
            {
                enemyBullet = null;
                EnemyBulletDestroyed?.Invoke();
            }
            else
            {
                enemyBullet = bullet;
                EnemyBulletMoved?.Invoke(bullet);
            }   
        }
    }

    private void UpdateEnemiesPos(float deltaTime)
    {
        float moveStep = enemySpeed * deltaTime;
        bool needDrop = false;

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
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i] += Coord.Down();               // actualiza modelo
                
            }
            EnemyMoved?.Invoke(enemies);
            return; // não mover horizontalmente neste frame
        }

        // 3) Caso contrário, move horizontalmente
        Coord step = (enemyMoveRight ? Coord.Right() : Coord.Left()) * moveStep;

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i] += step;
        }
        EnemyMoved?.Invoke(enemies);
    }
    /// <summary>
    /// Verifica colisões bala‑inimigo e actualiza listas / eventos.
    /// Considera hit se a distância em X e Y for < 0.5 unidades.
    /// </summary>
    private void DetectColision()
    {
        for (int b = bullets.Count - 1; b >= 0; b--)
        {
            Coord bulletPos = bullets[b];

            for (int e = enemies.Count - 1; e >= 0; e--)
            {
                Coord enemyPos = enemies[e];

                if (MathF.Abs(bulletPos.x - enemyPos.x) < 1f &&
                    MathF.Abs(bulletPos.y - enemyPos.y) < 0.5f)
                {
                    // Remove bala

                    bullets.RemoveAt(b);
                    BulletDestroyed?.Invoke(b);

                    // Regista hit no inimigo
                    EnemyHit(e);

                    break; // bala destruída, sai do loop interno
                }
            }
        }
    }

    private void DetectEnemyBulletColision()
    {
        if (enemyBullet.HasValue)
        {
            Coord bullet = enemyBullet.Value;
            if (MathF.Abs(bullet.x - playerPosition.x) < 1.2f &&
                    MathF.Abs(bullet.y - playerPosition.y) < 0.5f)
            {
                // Remove bala

                enemyBullet = null;
                EnemyBulletDestroyed?.Invoke();

                // Regista hit no inimigo
                bullets.Clear();
                enemies.Clear();
                OnGameOver?.Invoke();
            }

        }
    }

    private void DetectEnemyColision()
    {
        for (int e = enemies.Count - 1; e >= 0; e--)
        {
            Coord enemyPos = enemies[e];

            if (MathF.Abs(enemyPos.x - playerPosition.x) < 1f &&
                MathF.Abs(enemyPos.y - playerPosition.y) < 2f)
            {
                OnGameOver?.Invoke();
                break;
            }
        }   
    }

    private void EnemyHit(int index)
    {
        score += 10;
        enemies.RemoveAt(index);
        OnScoreChanged?.Invoke(score);
        OnEnemyKilled?.Invoke(index);


        if (enemies.Count == 0)
        {
            enemySpeed += 2f;
            SpawnEnemies();
        }
    }

    private bool GameOverCheck()
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
