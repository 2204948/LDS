using System.Collections.Generic;

// Definições dos delegates usados para eventos do modelo
public delegate void PositionChangedHandler(Coord newPosition);
public delegate void BulletFiredHandler(Coord bulletPosition);
public delegate void BulletMovedHandler(List<Coord> newBulletPosition);
public delegate void BulletDestroyedHandler(int bullet);
public delegate void EnemySpawnHandler(List<Coord> enemyPosition);
public delegate void EnemyMovHandler(List<Coord> newEnemyPosition);
public delegate void EnemyKilledHandler(int enemy);
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
