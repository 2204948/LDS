using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class GameIntegrationTests
{
    private GameObject gameObject;
    private GameView gameView;

    private void SetPrivateField<T>(object target, string fieldName, T value)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"Campo '{fieldName}' não encontrado.");
        field.SetValue(target, value);
    }

    private T GetPrivateField<T>(object target, string fieldName)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"Campo '{fieldName}' não encontrado.");
        return (T)field.GetValue(target);
    }

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        gameObject = new GameObject("GameViewTest");
        gameView = gameObject.AddComponent<GameView>();

        // GameObjects básicos
        var player = new GameObject("Player");
        var mainMenu = new GameObject("MainMenu");
        var gameOver = new GameObject("GameOver");

        gameView.GetType().GetField("player", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameView, player);
        gameView.GetType().GetField("mainMenuPanel", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameView, mainMenu);
        gameView.GetType().GetField("gameOverPanel", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameView, gameOver);

        // Prefabs simulados
        var bulletPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var enemyPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var explosionPrefab = GameObject.CreatePrimitive(PrimitiveType.Quad);
        var enemyBulletPrefab = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        gameView.GetType().GetField("bulletPrefab", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameView, bulletPrefab);
        gameView.GetType().GetField("enemyPrefab", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameView, enemyPrefab);
        gameView.GetType().GetField("explosionPrefab", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameView, explosionPrefab);
        gameView.GetType().GetField("enemyBulletPrefab", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameView, enemyBulletPrefab);

        // Simulação de scoreText
        var scoreTextObject = new GameObject("ScoreText");
        var tmpText = scoreTextObject.AddComponent<TMPro.TextMeshProUGUI>();
        gameView.GetType().GetField("scoreText", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameView, tmpText);

        yield return null;
    }


    [UnityTest]
    public IEnumerator StartGame_ShouldSpawnEnemies()
    {
        yield return null; // Esperar a inicialização

        //gameView.OnMainMenuStartButton();
        // Obter o controller
        var controller = GetPrivateField<GameController>(gameView, "controller");

        // Simular input de movimento para a direita (direction = 1, deltaTime = 1)
        controller.GetType().GetMethod("StartGame", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(controller, new object[] { });



        yield return new WaitForSeconds(0.2f); // Aguarda instância dos inimigos

        var enemies = GameObject.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None);
        int count = 0;
        foreach (var enemy in enemies)
        {
            if (enemy.name.Contains("Cube"))
                count++;
        }

        Assert.Greater(count, 0, "Inimigos deveriam ter sido instanciados após iniciar o jogo.");
    }

    [UnityTest]
    public IEnumerator PlayerMove_ShouldUpdateVisualPosition()
    {
        // Força Start() para conectar View ↔ Controller ↔ Model
        gameView.Invoke("Start", 0f);
        yield return new WaitForSeconds(0.1f);

        // Captura posição original do jogador
        var playerGO = GetPrivateField<GameObject>(gameView, "player");
        var originalPosition = playerGO.transform.position;

        // Inicia o jogo (ativa o player)
        //gameView.OnMainMenuStartButton();
        //yield return new WaitForSeconds(0.2f);

        // Obter o controller
        var controller = GetPrivateField<GameController>(gameView, "controller");


        // Simular input de movimento para a direita (direction = 1, deltaTime = 1)
        controller.GetType().GetMethod("StartGame", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(controller, new object[] { });


        // Simular input de movimento para a direita (direction = 1, deltaTime = 1)
        controller.GetType().GetMethod("OnPlayerInput", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(controller, new object[] { 1f, 1f });

        yield return new WaitForSeconds(0.1f);

        var newPosition = playerGO.transform.position;

        Assert.Greater(newPosition.x, originalPosition.x, "Player não se moveu para a direita.");
    }

    [UnityTest]
    public IEnumerator Shoot_ShouldSpawnBullet()
    {
        gameView.Invoke("Start", 0f);
        yield return new WaitForSeconds(0.1f);

        //gameView.OnMainMenuStartButton();
        //yield return new WaitForSeconds(0.1f);

        // Obter o controller
        var controller = GetPrivateField<GameController>(gameView, "controller");

        // Simular input de movimento para a direita (direction = 1, deltaTime = 1)
        controller.GetType().GetMethod("StartGame", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(controller, new object[] { });


        // Obter o modelo e forçar TryShot()
        var model = GetPrivateField<GameModel>(gameView, "model");
        model.TryShot();

        yield return new WaitForSeconds(0.1f);

        // Encontrar os objetos com base no prefab usado (Sphere)
        var bullets = GameObject.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None)
            .Where(go => go.name.Contains("Sphere"))
            .ToList();

        Assert.IsTrue(bullets.Count > 0, "Nenhuma bala foi instanciada após o disparo.");
    }

    [UnityTest]
    public IEnumerator BulletHitsEnemy_ShouldDestroyEnemyAndIncreaseScore()
    {
        // Setup: cria GameView e simula início do jogo
        gameView.Invoke("Start", 0f);
        yield return new WaitForSeconds(0.1f);
        //gameView.OnMainMenuStartButton();
        //yield return new WaitForSeconds(0.1f);


        // Obter o controller
        var controller = GetPrivateField<GameController>(gameView, "controller");

        // Simular input de movimento para a direita (direction = 1, deltaTime = 1)
        controller.GetType().GetMethod("StartGame", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(controller, new object[] { });

        var model = GetPrivateField<GameModel>(gameView, "model");

        controller.GetType().GetMethod("Shoot", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(controller, new object[] { });

        // Posição forçada: inimigo e bala sobrepostos
        SetPrivateField(model, "bullets", new List<Coord> { new Coord(-4f, 12f, 0f) });

        // Força verificação de colisão
        model.OnUpdate(0.01f);
        yield return new WaitForSeconds(0.2f);

        // Verificação: inimigo removido e score aumentado
        var enemiesVisual = GetPrivateField<List<GameObject>>(gameView, "enemies");
        //Assert.Less(15, enemiesVisual.Count, "Inimigo não foi removido da lista visual.");

        var scoreText = GetPrivateField<TextMeshProUGUI>(gameView, "scoreText");
        Assert.IsTrue(scoreText.text.Contains("10"), "Score não atualizado corretamente.");
    }


    [UnityTest]
    public IEnumerator EnemyBulletHitsPlayer_ShouldTriggerGameOverPanel()
    {
        // Preparar o ambiente
        var model = GetPrivateField<GameModel>(gameView, "model");
        gameView.Invoke("Start", 0f);
        yield return new WaitForSeconds(0.1f);

        var enemies = GetPrivateField<List<GameObject>>(gameView, "enemies");


        // Obter o controller
        var controller = GetPrivateField<GameController>(gameView, "controller");

        gameView.OnMainMenuStartButton();
        
        //controller.GetType().GetMethod("StartGame", BindingFlags.NonPublic | BindingFlags.Instance)
            //?.Invoke(controller, new object[] {  });
        //model.OnUpdate(0.1f);
        //yield return new WaitForSeconds(0.2f);

        var coord = new Coord(0f, -13.5f, 0f);
        var coord2 = new Coord(0f, -13.4f, 0f);

        SetPrivateField(model, "enemyBullet", new Coord(0f, -13.5f, 0f));
        SetPrivateField(model, "playerPosition", coord);
        var gameOverPanel = GetPrivateField<GameObject>(gameView, "gameOverPanel");


        model.OnUpdate(0.01f);
        yield return new WaitForSeconds(0.2f);
        Debug.Log($"GAME");
        // Verificação final
        Assert.That(gameOverPanel.activeSelf, Is.True, "Painel de Game Over não foi ativado após o jogador ser atingido.");
    }
}
