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
public delegate void EnemyBulletFiredHandler(Coord bulletPosition);
public delegate void EnemyBulletMovedHandler(Coord bulletPosition);
public delegate void EnemyBulletDestroyedHandler();
public delegate void ClearPlayerBulletsHandler();


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
    event EnemyBulletFiredHandler EnemyBulletFired;
    event EnemyBulletMovedHandler EnemyBulletMoved;
    event EnemyBulletDestroyedHandler EnemyBulletDestroyed;
    event ClearPlayerBulletsHandler ClearPlayerBullets;


    // Métodos públicos para interação com o modelo
    void StartNewGame();
    void Move(float direction, float deltaTime);
    void TryShot();
    void OnUpdate(float deltaTime);
}
