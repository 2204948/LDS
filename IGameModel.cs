using System;

// Definições dos delegates usados para eventos do modelo
public delegate void PositionChangedHandler(Coord newPosition);
public delegate void BulletFiredHandler(Coord bulletPosition);
public delegate void BulletMovedHandler(Coord newBulletPosition);
public delegate void BulletDestroyedHandler(Coord bulletPosition);
public delegate void EnemySpawnHandler(Coord enemyPosition);
public delegate void EnemyMovHandler(Coord newEnemyPosition, bool moveDir);
public delegate void EnemyKilledHandler(Coord enemyPosition);
public delegate void ScoreChangedHandler(int newScore);
public delegate void GameOverHandler();

public interface IGameModel
{
    // Eventos emitidos pelo modelo
    event PositionChangedHandler OnPositionChanged;
    event BulletFiredHandler BulletFired;
    event BulletMovedHandler BulletMoved;
    event BulletDestroyedHandler BulletDestroyed;
    event EnemySpawnHandler OnEnemySpawn;
    event EnemyMovHandler EnemyMoved;
    event EnemyKilledHandler OnEnemyKilled;
    event ScoreChangedHandler OnScoreChanged;
    event GameOverHandler OnGameOver;

    // Métodos públicos para interação com o modelo
    void StartNewGame();
    void Move(float direction, float deltaTime);
    void TryShot();
    void OnUpdate(float deltaTime);
}