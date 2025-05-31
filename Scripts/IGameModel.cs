using System.Collections.Generic;

/// <summary>
/// Delegados (tipos de função) para eventos emitidos pelo modelo.
/// Permitem à View reagir a mudanças de estado.
/// </summary>

// Jogador
public delegate void PositionChangedHandler(Coord newPosition);

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


/// <summary>
/// Interface do modelo. Expõe eventos e métodos para interação com o jogo.
/// </summary>
public interface IGameModel
{
    // === Eventos ===

    // Jogador
    event PositionChangedHandler OnPositionChanged;

    // Balas do jogador
    event BulletFiredHandler BulletFired;
    event BulletMovedHandler BulletMoved;
    event BulletDestroyedHandler BulletDestroyed;
    event ClearPlayerBulletsHandler ClearPlayerBullets;

    // Inimigos
    event EnemySpawnHandler OnEnemySpawn;
    event EnemyMovHandler EnemyMoved;
    event EnemyKilledHandler OnEnemyKilled;

    // Pontuação e fim de jogo
    event ScoreChangedHandler OnScoreChanged;
    event GameOverHandler OnGameOver;

    // Balas inimigas
    event EnemyBulletFiredHandler EnemyBulletFired;
    event EnemyBulletMovedHandler EnemyBulletMoved;
    event EnemyBulletDestroyedHandler EnemyBulletDestroyed;

    // === Métodos ===

    /// <summary>Inicia um novo jogo, reiniciando o estado interno.</summary>
    void StartNewGame();

    /// <summary>Move o jogador numa direção, com base no tempo decorrido.</summary>
    void Move(float direction, float deltaTime);

    /// <summary>Solicita o disparo de uma bala pelo jogador.</summary>
    void TryShot();

    /// <summary>Atualiza a lógica do jogo (chamado a cada frame).</summary>
    void OnUpdate(float deltaTime);
}
