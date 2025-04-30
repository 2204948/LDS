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
        view.StartGame();// view.ShowMainMenu()
        model.StartNewGame();
    }

    public void OnPlayerInput(float direction, float deltaTime)
    {
        model.Move(direction, deltaTime);
    }

    public void Shoot(bool shoot)
    {
        model.TryShot();
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
        model.StartNewGame();
        view.StartGame();
    }

    public void ShowMenu()
    {
        view.ShowMainMenu();
    }

    public void OnUpdate(float deltatime)
    {
        model.OnUpdate(deltaTime);
    }
}
