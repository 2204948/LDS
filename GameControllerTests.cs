using NUnit.Framework;


public class MockGameView : IGameView
{
    public event OnMoveHandler OnMove;
    public event OnShootHandler OnShoot;
    public event OnMainMenuStartHandler OnMainMenuStart;
    public event OnGameOverStartHandler OnGameOverStart;
    public event OnGameOverQuitHandler OnGameOverQuit;
    public event OnMainMenuExitHandler OnMainMenuExit;
    public event UpdateHandler OnUpdate;

    public bool showMainMenuCalled = false;
    public bool startGameCalled = false;

    public void ShowMainMenu() => showMainMenuCalled = true;
    public void StartGame() => startGameCalled = true;
    public void ShowGameOver() { }
    public void UpdateScore(int score) { }
    public void HandleEnemyKilled(int enemy) { }

    public void TriggerMove(float direction, float deltaTime)
    {
        OnMove?.Invoke(direction, deltaTime);
    }

    public void TriggerShoot()
    {
        OnShoot?.Invoke();
    }

    public void TriggerShowMenu()
    {
        OnGameOverQuit?.Invoke();
    }

    public void TriggerStartOnMain()
    {
        OnMainMenuStart?.Invoke();
    }

    public void TriggerStartOnGameOver()
    {
        OnGameOverStart?.Invoke();
    }
}

public class MockGameModel : GameModel
{
    public bool startCalled = false;
    public bool moveCalled = false;
    public bool shotCalled = false;
    public bool updateCalled = false;
    public float lastDelta;

    public override void OnUpdate(float deltaTime)
    {
        updateCalled = true;
        lastDelta = deltaTime;
    }

    public override void StartNewGame() => startCalled = true;
    public override void Move(float direction, float deltaTime) => moveCalled = true;
    public override void TryShot() => shotCalled = true;
}

public class GameControllerTests
{
    [Test]
    public void StartGame_CallsViewAndModel()
    {
        var view = new MockGameView();
        var model = new MockGameModel();

        var controller = new GameController(view, model);
        view.TriggerStartOnMain();

        Assert.IsTrue(view.startGameCalled, "StartGame da View não foi chamado");
        Assert.IsTrue(model.startCalled, "StartNewGame do Model não foi chamado");
    }

    [Test]
    public void OnPlayerInput_CallsModelMove()
    {
        var view = new MockGameView();
        var model = new MockGameModel();

        var controller = new GameController(view, model);
        
        view.TriggerMove(1f, 0.016f);

        Assert.IsTrue(model.moveCalled, "Move do Model não foi chamado");
    }

    [Test]
    public void Shoot_CallsModelTryShot()
    {
        var view = new MockGameView();
        var model = new MockGameModel();

        var controller = new GameController(view, model);
        view.TriggerShoot();

        Assert.IsTrue(model.shotCalled, "TryShot do Model não foi chamado");
    }

    [Test]
    public void ShowMenu_ShouldCallViewShowMainMenu()
    {
        var view = new MockGameView();
        var model = new MockGameModel();
        var controller = new GameController(view, model);

        view.TriggerShowMenu();

        Assert.IsTrue(view.showMainMenuCalled, "ShowMainMenu da view não foi chamado.");
    }

    [Test]
    public void OnUpdate_ShouldCallModelUpdateWithCorrectDelta()
    {
        var view = new MockGameView();
        var model = new MockGameModel();
        var controller = new GameController(view, model);

        controller.OnUpdate(0.016f);

        Assert.IsTrue(model.updateCalled, "OnUpdate do modelo não foi chamado.");
        Assert.AreEqual(0.016f, model.lastDelta, 0.0001f, "Delta passado para o modelo está incorreto.");
    }

    [Test]
    public void StartGameOnGameOver_CallsViewAndModel()
    {
        var view = new MockGameView();
        var model = new MockGameModel();

        var controller = new GameController(view, model);
        view.TriggerStartOnGameOver();

        Assert.IsTrue(view.startGameCalled, "StartGame da View não foi chamado");
        Assert.IsTrue(model.startCalled, "StartNewGame do Model não foi chamado");
    }
}