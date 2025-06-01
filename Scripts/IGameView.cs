using System;

/// <summary>
/// Delegados para input do utilizador, recebidos pela View e transmitidos ao controlador.
/// </summary>
public delegate void OnMoveHandler(float horizontal, float deltaTime);
public delegate void OnShootHandler();
public delegate void OnMainMenuStartHandler();
public delegate void OnGameOverStartHandler();
public delegate void OnGameOverQuitHandler();
public delegate void OnMainMenuExitHandler();
public delegate void UpdateHandler(float deltaTime);

/// <summary>
/// Interface da View (interface gr�fica).
/// Define os eventos e m�todos que permitem comunica��o com o controlador.
/// </summary>
public interface IGameView
{
    // === Eventos (input do jogador) ===

    /// <summary>Movimento horizontal (esquerda/direita).</summary>
    event OnMoveHandler OnMove;

    /// <summary>Pedido de disparo (ex: tecla espaço).</summary>
    event OnShootHandler OnShoot;

    /// <summary>In�cio do jogo a partir do menu principal.</summary>
    event OnMainMenuStartHandler OnMainMenuStart;

    event OnMainMenuExitHandler OnMainMenuExit;

    /// <summary>Recome�ar o jogo ap�s Game Over.</summary>
    event OnGameOverStartHandler OnGameOverStart;

    /// <summary>Voltar ao menu ap�s Game Over.</summary>
    event OnGameOverQuitHandler OnGameOverQuit;

    event UpdateHandler OnUpdate;

    // === M�todos visuais (feedback para o jogador) ===

    /// <summary>Mostra o menu principal.</summary>
    void ShowMainMenu();

    /// <summary>Inicia o jogo (esconde menus e ativa player).</summary>
    void StartGame();

    /// <summary>Mostra o painel de Game Over.</summary>
    void ShowGameOver();

    /// <summary>Atualiza o score apresentado no ecr�.</summary>
    void UpdateScore(int score);

    /// <summary>Lida com a morte de um inimigo (ex: explos�o).</summary>
    void HandleEnemyKilled(int enemy);
}
