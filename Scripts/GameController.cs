public class GameController
{
    private IGameModel model;
    private GameView view;

    public GameController(GameView view, GameModel model)
    {
        this.view = view;
        this.model = model;
        view.OnMove += OnPlayerInput;
        view.OnShoot += Shoot;
        view.OnMainMenuStart += StartGame;
        view.OnGameOverStart += StartGame;
        view.OnGameOverQuit += ShowMenu;
        view.ShowMainMenu();

    }

    public void OnPlayerInput(float direction, float deltaTime)
    {
        model.Move(direction, deltaTime);
    }

    //Tiro:
    public void Shoot()
    {
        model.TryShot();
    }

    public void StartGame()
    {
        model.StartNewGame();
        view.StartGame();
    }

    public void ShowMenu()
    {
        view.ShowMainMenu();
    }

    public void OnUpdate(float deltaTime)
    {
        model.OnUpdate(deltaTime);
    }
}
