using System;
using UnityEngine;

/// <summary>
/// Classe controladora. Interliga input da View com lógica do Modelo.
/// </summary>
public class GameController
{
    private IGameModel model;
    private GameView view;

    private float lastShotTime = -1f;
    private const float bulletCooldown = 0.5f;

    /// <summary>
    /// Construtor principal. Liga a View ao Modelo.
    /// </summary>
    public GameController(GameView view, GameModel model)
    {
        this.view = view;
        this.model = model;

        // Liga eventos da View aos métodos do controlador
        view.OnMove += OnPlayerInput;
        view.OnShoot += Shoot;
        view.OnMainMenuStart += StartGame;
        view.OnGameOverStart += StartGame;
        view.OnGameOverQuit += ShowMenu;

        // Mostra o menu inicial
        view.ShowMainMenu();
    }

    /// <summary>
    /// Move o jogador com base no input horizontal.
    /// </summary>
    public void OnPlayerInput(float direction, float deltaTime)
    {
        model.Move(direction, deltaTime);
    }

    /// <summary>
    /// Lida com pedido de disparo. Aplica cooldown entre tiros.
    /// </summary>
    public void Shoot()
    {
        float time = Time.time;
        if (time - lastShotTime >= bulletCooldown)
        {
            lastShotTime = time;
            model.TryShot();
        }
    }

    /// <summary>
    /// Começa um novo jogo.
    /// </summary>
    public void StartGame()
    {
        model.StartNewGame();
        view.StartGame();
        Debug.Log("[Info] Jogo iniciado.");
    }

    /// <summary>
    /// Mostra o menu principal.
    /// </summary>
    public void ShowMenu()
    {
        view.ShowMainMenu();
        Debug.Log("[Info] Menu principal exibido.");
    }

    /// <summary>
    /// Atualização chamada a cada frame.
    /// </summary>
    public void OnUpdate(float deltaTime)
    {
        model.OnUpdate(deltaTime);
    }
}
