using UnityEngine;

public class GameController
{
    private GameModel model;
    private GameView view;

    private float lastShotTime = -1f;
    private const float bulletCooldown = 0.5f;

    public GameController(GameView view)
    {
        this.view = view;
        this.model = new GameModel();

        // Ligar eventos do Model à View
        model.OnPositionChanged += view.UpdatePlayerPosition;
        model.BulletFired += view.SpawnBullet;
        model.BulletMoved += view.MoveBullet;
        model.BulletDestroyed += view.DestroyBullet;
        model.OnEnemySpawn += view.SpawnEnemy;
        model.OnEnemyKilled += view.ShowExplosion;
        model.OnGameOver += view.ShowGameOver;

        // Inicializar o jogo
        model.StartNewGame();
        view.UpdateScore(0);
    }

    public void OnPlayerInput(float direction, bool shoot)
    {
        model.Move(direction, Time.deltaTime);

        if (shoot && Time.time - lastShotTime >= bulletCooldown)
        {
            lastShotTime = Time.time;
            model.TryShot();
        }
    }

    public void Update()
    {
        model.Update(Time.deltaTime);
    }

    public void OnEnemyKilled(Vector2 enemyPosition)
    {
        model.EnemyHit(enemyPosition);
    }

    public void SpawnEnemy(Vector2 position)
    {
        model.EnemySpawn(position);
    }

    public void RestartGame()
    {
        model.StartNewGame();
        view.UpdateScore(0);
        view.StartGame();
    }
}
