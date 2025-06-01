using System;
using UnityEngine;

/// <summary>
/// Classe controladora. Interliga input da View com l�gica do Modelo.
/// </summary>
public class GameController
{
    private GameModel model;
    private IGameView view;


    /// <summary>
    /// Construtor principal. Liga a View ao Modelo.
    /// </summary>
    public GameController(IGameView view, GameModel model)
    {
        this.view = view;
        this.model = model;

        // Liga eventos da View aos m�todos do controlador
        view.OnMove += OnPlayerInput;
        view.OnShoot += Shoot;
        view.OnMainMenuStart += StartGame;
        view.OnMainMenuExit += ExitGame;
        view.OnGameOverStart += StartGame;
        view.OnGameOverQuit += ShowMenu;
        view.OnUpdate += OnUpdate;
    }

    /// <summary>
    /// Move o jogador com base no input horizontal.
    /// </summary>
    private void OnPlayerInput(float direction, float deltaTime)
    {
        model.Move(direction, deltaTime);
    }

    /// <summary>
    /// Lida com pedido de disparo. Aplica cooldown entre tiros.
    /// </summary>
    private void Shoot()
    {
        model.TryShot();
    }

    /// <summary>
    /// Come�a um novo jogo.
    /// </summary>
    private void StartGame()
    {
        model.StartNewGame();
        view.StartGame();
        Debug.Log("[Info] Jogo iniciado.");
    }

    /// <summary>
    /// Mostra o menu principal.
    /// </summary>
    private void ShowMenu()
    {
        view.ShowMainMenu();
        Debug.Log("[Info] Menu principal exibido.");
    }

    private void ExitGame()
    {
        model.ExitGame();
    }
    
    /// <summary>
    /// Atualiza��o chamada a cada frame.
    /// </summary>
    public void OnUpdate(float deltaTime)
    {
        model.OnUpdate(deltaTime);
    }
}
