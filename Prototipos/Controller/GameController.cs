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


        // Inicializar o jogo
        model.StartNewGame();
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
        model.StartNewGame()
        view.StartGame();
    }
}
