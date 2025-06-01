using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Modelo principal do jogo. Gere lógica interna do jogador, inimigos, tiros e colisões.
/// Comunica com a View através de eventos.
/// </summary>


/// Delegados (tipos de função) para eventos emitidos pelo modelo.
/// Permitem à View reagir a mudanças de estado.

// Jogador
public delegate void PositionChangedHandler(Coord newPosition);
public delegate void ExitGameHandler();

// Balas do jogador
public delegate void BulletFiredHandler(Coord bulletPosition);
public delegate void BulletMovedHandler(List<Coord> newBulletPositions);
public delegate void BulletDestroyedHandler(int bulletIndex);
public delegate void ClearPlayerBulletsHandler();

// Inimigos
public delegate void EnemySpawnHandler(List<Coord> enemyPositions);
public delegate void EnemyMovHandler(List<Coord> newEnemyPositions);
public delegate void EnemyKilledHandler(int enemyIndex);

// Pontuação e estado do jogo
public delegate void ScoreChangedHandler(int newScore);
public delegate void GameOverHandler();

// Balas dos inimigos
public delegate void EnemyBulletFiredHandler(Coord bulletPosition);
public delegate void EnemyBulletMovedHandler(Coord bulletPosition);
public delegate void EnemyBulletDestroyedHandler();

public class GameModel : IGameModel
{
    // === Eventos enviados para a View reagir visualmente ===
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
    public event ClearPlayerBulletsHandler ClearPlayerBullets;
    public event ExitGameHandler OnExitGame;

    // === Estado do jogador ===
    private Coord playerPosition;

    // === Estado do jogo ===
    private List<Coord> bullets = new();       // Balas disparadas pelo jogador
    private List<Coord> enemies = new();       // Inimigos vivos
    private Coord? enemyBullet;                // Única bala inimiga ativa (pode ser null)

    // === Constantes e limites do jogo ===
    private const float playerMovSpeed = 8.0f; // Velocidade de movimento do jogador
    private const float bulletSpeed = 10f;     // Velocidade das balas
    private float enemySpeed = 5f;             // Velocidade dos inimigos (acelera com o tempo)

    private readonly float maxLeft = -14f, maxRight = 14f;
    private readonly float MaxY = 18f, minY = -14f;

    private float lastShotTime = -1f;
    private const float bulletCooldown = 0.5f;

    private bool enemyMoveRight = true; // Direção atual dos inimigos
    private int score = 0;              // Pontuação
    private bool game;                  // Estado do jogo (ativo ou não)

    /// <summary>
    /// Começa um novo jogo: reinicia estado e gera inimigos.
    /// </summary>
    public virtual void StartNewGame()
    {
        bullets.Clear();
        enemies.Clear();
        enemySpeed = 5f;
        score = 0;
        enemyBullet = null;
        playerPosition = new Coord(0f, -13.5f, 0f); // Jogador no centro, em baixo
        game = true;

        SpawnEnemies(); // Cria inimigos iniciais

        // Notifica a View
        OnScoreChanged?.Invoke(score);
        OnPositionChanged?.Invoke(playerPosition);
    }

    /// <summary>
    /// Gera inimigos em formação, em várias linhas e colunas.
    /// </summary>
    private void SpawnEnemies()
    {
        int rows = 3, columns = 5;
        float startX = -8f, startY = 14f;
        float spacingX = 4f, spacingY = 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float x = startX + (col * spacingX);
                float y = startY - (row * spacingY);
                enemies.Add(new Coord(x, y, 0));
            }
        }

        OnEnemySpawn?.Invoke(enemies); // Notifica a View
    }

    /// <summary>
    /// Atualiza a lógica do jogo a cada frame.
    /// Só executa se o jogo estiver ativo.
    /// </summary>
    public virtual void OnUpdate(float deltaTime)
    {
        if (!game) return;

        UpdateBulletPos(deltaTime);
        UpdateEnemiesPos(deltaTime);
        DetectColision();
        UpdateEnemyBulletPos(deltaTime);

        if (enemyBullet == null)
            enemyShot();

        DetectEnemyBulletColision();
        DetectEnemyColision();
    }

    /// <summary>
    /// Move o jogador na horizontal, respeitando os limites.
    /// </summary>
    public virtual void Move(float direction, float deltaTime)
    {
        float deltaX = direction * playerMovSpeed * deltaTime;
        playerPosition.x = Coord.Clamp(playerPosition.x + deltaX, maxLeft, maxRight);
        OnPositionChanged?.Invoke(playerPosition);
    }

    /// <summary>
    /// Dispara uma bala a partir da posição atual do jogador.
    /// </summary>
    public virtual void TryShot()
    {
        float time = Time.time;
        if (time - lastShotTime >= bulletCooldown)
        {
            lastShotTime = time;
            Coord bulletPos = playerPosition + new Coord(0, 1, 0);
            bullets.Add(bulletPos);
            BulletFired?.Invoke(bulletPos);
        }
    }

    /// <summary>
    /// Inimigo aleatório dispara uma bala.
    /// </summary>
    public void enemyShot()
    {
        if (enemies.Count == 0)
        {
            Console.WriteLine("ERRO: Nenhum inimigo disponível para disparar.");
            return;
        }

        int index = new System.Random().Next(enemies.Count);
        Coord bulletPos = enemies[index] + new Coord(0, -1, 0);
        enemyBullet = bulletPos;
        EnemyBulletFired?.Invoke(bulletPos);
    }

    /// <summary>
    /// Atualiza posições das balas do jogador e remove as que saem do ecrã.
    /// </summary>
    private void UpdateBulletPos(float deltaTime)
    {
        int i = 0;
        while (i < bullets.Count)
        {
            bullets[i] += Coord.Up(bulletSpeed * deltaTime);

            if (bullets[i].y > MaxY)
            {
                bullets.RemoveAt(i);
                BulletDestroyed?.Invoke(i);
                // Não incrementa i, porque o próximo elemento deslizou para a posição i
            }
            else
            {
                i++; // Só avança se não houve remoção
            }
        }

        BulletMoved?.Invoke(bullets);
    }


    /// <summary>
    /// Atualiza posição da bala inimiga. Remove se sair do ecrã.
    /// </summary>
    private void UpdateEnemyBulletPos(float deltaTime)
    {
        if (!enemyBullet.HasValue) return;

        Coord bullet = enemyBullet.Value + Coord.Down(bulletSpeed * deltaTime);
        if (bullet.y < minY)
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

    /// <summary>
    /// Move os inimigos na horizontal e vertical se atingirem os limites.
    /// </summary>
    private void UpdateEnemiesPos(float deltaTime)
    {
        float step = enemySpeed * deltaTime;
        bool needDrop = false;

        foreach (var enemy in enemies)
        {
            float nextX = enemy.x + (enemyMoveRight ? step : -step);
            if (nextX < maxLeft || nextX > maxRight)
            {
                needDrop = true;
                enemyMoveRight = !enemyMoveRight;
                break;
            }
        }

        if (needDrop)
        {
            for (int i = 0; i < enemies.Count; i++)
                enemies[i] += Coord.Down(); // Desce uma linha
            EnemyMoved?.Invoke(enemies);
            return;
        }

        Coord direction = (enemyMoveRight ? Coord.Right() : Coord.Left()) * step;
        for (int i = 0; i < enemies.Count; i++)
            enemies[i] += direction;

        EnemyMoved?.Invoke(enemies);
    }

    /// <summary>
    /// Verifica se alguma bala do jogador colidiu com algum inimigo.
    /// </summary>
    private void DetectColision()
    {
        for (int b = 0; b < bullets.Count; b++)
        {
            Coord bulletPos = bullets[b];

            for (int e = 0; e < enemies.Count; e++)
            {
                Coord enemyPos = enemies[e];

                if (MathF.Abs(bulletPos.x - enemyPos.x) < 1f &&
                    MathF.Abs(bulletPos.y - enemyPos.y) < 0.5f)
                {
                    bullets.RemoveAt(b);
                    BulletDestroyed?.Invoke(b);
                    EnemyHit(e);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Verifica se a bala inimiga colidiu com o jogador.
    /// </summary>
    private void DetectEnemyBulletColision()
    {
        if (!enemyBullet.HasValue) return;

        Coord bullet = enemyBullet.Value;
        if (MathF.Abs(bullet.x - playerPosition.x) < 1.2f &&
            MathF.Abs(bullet.y - playerPosition.y) < 0.5f)
        {
            enemyBullet = null;
            game = false;
            EnemyBulletDestroyed?.Invoke();

            bullets.Clear();
            enemies.Clear();
            OnGameOver?.Invoke();
        }
    }

    /// <summary>
    /// Verifica se algum inimigo colidiu diretamente com o jogador.
    /// </summary>
    private void DetectEnemyColision()
    {
        foreach (var enemy in enemies)
        {
            if (MathF.Abs(enemy.x - playerPosition.x) < 1f &&
                MathF.Abs(enemy.y - playerPosition.y) < 2f)
            {
                game = false;
                OnGameOver?.Invoke();
                break;
            }
        }
    }

    /// <summary>
    /// Lida com o acerto de um inimigo: remove-o e atualiza score.
    /// </summary>
    private void EnemyHit(int index)
    {
        if (index < 0 || index >= enemies.Count)
        {
            Console.WriteLine($"ERRO: EnemyHit: índice inválido {index} (tamanho da lista: {enemies.Count})");
            return;
        }


        score += 10;
        enemies.RemoveAt(index);
        OnScoreChanged?.Invoke(score);
        OnEnemyKilled?.Invoke(index);

        // Quando todos os inimigos são mortos, próxima ronda
        if (enemies.Count == 0)
        {
            enemySpeed += 2f;
            bullets.Clear();
            ClearPlayerBullets?.Invoke();
            SpawnEnemies();
        }
    }

    public void ExitGame()
    {
        //Possível lógica adicional para guardar dados do jogo.
        OnExitGame?.Invoke();
    }
}
