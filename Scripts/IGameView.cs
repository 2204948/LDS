using System;

public delegate void OnMoveHandler(float horizontal, float deltaTime);
public delegate void OnShootHandler();
public delegate void OnMainMenuStartHandler();
public delegate void OnGameOverStartHandler();
public delegate void OnGameOverQuitHandler();

public interface IGameView
{
    // Input do utilizador
    public event OnMoveHandler OnMove;
    public event OnShootHandler OnShoot;
    public event OnMainMenuStartHandler OnMainMenuStart;
    public event OnGameOverStartHandler OnGameOverStart;
    public event OnGameOverQuitHandler OnGameOverQuit;

    // Feedback visual
    void ShowMainMenu();
    void StartGame();
    void ShowGameOver();
    void UpdateScore(int score);
    void FlashPlayerDamage();
    void HandleEnemyKilled(Coord pos);
}
