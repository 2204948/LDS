using UnityEngine;

public class GameController
{
    private GameModel model;
    private GameView view;

    private float lastShotTime = -1f;
    private const float bulletCooldown = 0.5f;

    public GameController(GameView view, GameModel model)
    {
        this.view = view;
        this.model = model;
        view.ShowMainMenu();
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

    public void StartGame()
    {
        model.StartNewGame();
    }

    public void RestartGame()
    {
        model.StartNewGame()
        view.StartGame();
    }

    public void ShowMenu()
    {
        view.ShowMainMenu();
    }
}
