using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Classe responsável pela interface gráfica e interações com o jogador.
/// Gera, atualiza e destrói objetos visuais com base nos eventos do Modelo.
/// </summary>
public class GameView : MonoBehaviour, IGameView
{
    private GameController controller;
    private GameModel model;
    private bool game = false; // Se o jogo está ativo

    // === Referências definidas no Inspector (Unity Editor) ===
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI errorLog;

    // === Estado atual de objetos visuais ===
    private List<GameObject> bullets = new();
    private List<GameObject> enemies = new();
    private GameObject enemyBullet;

    // === Eventos para input do jogador ===
    public event OnMoveHandler OnMove;
    public event OnShootHandler OnShoot;
    public event OnMainMenuStartHandler OnMainMenuStart;
    public event OnGameOverStartHandler OnGameOverStart;
    public event OnGameOverQuitHandler OnGameOverQuit;
    public event OnMainMenuExitHandler OnMainMenuExit;
    public event UpdateHandler OnUpdate;

    /// <summary>
    /// Inicialização automática pela Unity. Conecta o modelo e subscreve os eventos.
    /// </summary>
    void Start()
    {
        model = new GameModel();

        // Subscrição dos eventos emitidos pelo modelo (lógica de jogo)
        model.OnPositionChanged += UpdatePlayerPosition;
        model.BulletFired += SpawnBullet;
        model.BulletMoved += MoveBullet;
        model.BulletDestroyed += DestroyBullet;
        model.OnEnemySpawn += SpawnEnemy;
        model.EnemyMoved += MoveEnemy;
        model.OnEnemyKilled += HandleEnemyKilled;
        model.OnGameOver += GameOver;
        model.OnScoreChanged += UpdateScore;
        model.EnemyBulletFired += SpawnEnemyBullet;
        model.EnemyBulletMoved += MoveEnemyBullet;
        model.EnemyBulletDestroyed += DestroyEnemyBullet;
        model.ClearPlayerBullets += ClearPlayerBullets;
        model.OnExitGame += ExitGame;
        model.OnError += DisplayError;

        // Cria o controlador, ligando View ao Modelo
        controller = new GameController(this, model);
        // Mostra o menu inicial
        ShowMainMenu();
    }



    /// <summary>
    /// Chamado automaticamente pela Unity a cada frame.
    /// Lê input do jogador e envia comandos para o controlador.
    /// </summary>
    private void Update()
    {
        if (!game)
            return; // Se o jogo não está ativo, não reage ao input

        controller.OnUpdate(Time.deltaTime);
        //OnUpdate?.Invoke(Time.deltaTime); // Atualiza o modelo com o tempo atual

        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
            OnMove?.Invoke(horizontal, Time.deltaTime);

        bool shoot = Input.GetKeyDown(KeyCode.Space);
        if (shoot)
            OnShoot?.Invoke();
    }


    /// <summary>
    /// Mostra o menu principal e oculta os restantes elementos.
    /// </summary>
    public void ShowMainMenu()
    {
        mainMenuPanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
        player?.SetActive(false);
        scoreText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Ativa o jogo (chamado ao iniciar ou reiniciar).
    /// </summary>
    public void StartGame()
    {
        // Oculta a mensagem de erro, se estiver visível
        if (errorLog != null)
            errorLog.gameObject.SetActive(false);

        mainMenuPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        player?.SetActive(true);
        scoreText.gameObject.SetActive(true);
        game = true;
    }

    /// <summary>
    /// Mostra o ecrã de fim de jogo.
    /// </summary>
    public void ShowGameOver()
    {
        mainMenuPanel?.SetActive(false);
        gameOverPanel?.SetActive(true);
        player?.SetActive(false);
    }

    /// <summary>
    /// Botão do menu principal: iniciar jogo.
    /// </summary>
    public void OnMainMenuStartButton()
    {
        OnMainMenuStart?.Invoke();
    }

    public void OnMainMenuExitButton()
    {
        OnMainMenuExit?.Invoke();
    }

    /// <summary>
    /// Botão de Game Over: voltar a jogar.
    /// </summary>
    public void OnGameOverStartButton()
    {
        OnGameOverStart?.Invoke();
    }

    /// <summary>
    /// Botão de Game Over: sair para menu.
    /// </summary>
    public void OnGameOverQuitButton()
    {
        OnGameOverQuit?.Invoke();
    }

    /// <summary>
    /// Atualiza a posição visual do jogador.
    /// </summary>
    private void UpdatePlayerPosition(Coord pos)
    {
        Vector3 vectorPos = CoordToVector(pos);
        if (player != null)
            player.transform.position = vectorPos;
    }

    /// <summary>
    /// Cria uma nova bala visual na posição indicada.
    /// </summary>
    private void SpawnBullet(Coord pos)
    {
        Vector3 vectorPos = CoordToVector(pos);
        var b = Instantiate(bulletPrefab, vectorPos, Quaternion.identity);
        bullets.Add(b);
    }

    /// <summary>
    /// Move todas as balas do jogador para as novas posições.
    /// Garante que o número de objetos visuais corresponde ao número de coordenadas.
    /// </summary>
    private void MoveBullet(List<Coord> newPos)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            Vector3 vectorPos = CoordToVector(newPos[i]);
            bullets[i].transform.position = vectorPos;
        }
    }

    /// <summary>
    /// Remove visualmente a bala indicada por índice.
    /// </summary>
    private void DestroyBullet(int bullet)
    {
        Destroy(bullets[bullet]);
        bullets.RemoveAt(bullet);
    }


    /// <summary>
    /// Cria a bala inimiga na posição indicada.
    /// </summary>
    private void SpawnEnemyBullet(Coord bulletPosition)
    {
        Vector3 vectorPos = CoordToVector(bulletPosition);
        var b = Instantiate(enemyBulletPrefab, vectorPos, Quaternion.identity);
        enemyBullet = b;
    }

    /// <summary>
    /// Move a bala inimiga se ela existir.
    /// </summary>
    private void MoveEnemyBullet(Coord bulletPosition)
    {
        Vector3 vectorPos = CoordToVector(bulletPosition);
        enemyBullet.transform.position = vectorPos;
    }

    /// <summary>
    /// Destrói a bala inimiga se ela existir.
    /// </summary>
    private void DestroyEnemyBullet()
    {
        if (enemyBullet != null)
        {
            Destroy(enemyBullet);
            enemyBullet = null;
        }
    }


    /// <summary>
    /// Cria inimigos nas posições indicadas pelo modelo.
    /// </summary>
    private void SpawnEnemy(List<Coord> enemyPosition)
    {
        foreach (Coord enemy in enemyPosition)
        {
            Vector3 vectorPos = CoordToVector(enemy);
            var e = Instantiate(enemyPrefab, vectorPos, Quaternion.identity);
            enemies.Add(e);
        }
    }

    /// <summary>
    /// Move os inimigos para novas posições.
    /// Garante que a lista de GameObjects e a lista de coordenadas estão sincronizadas.
    /// </summary>
    private void MoveEnemy(List<Coord> newPos)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Vector3 vectorPos = CoordToVector(newPos[i]);
            enemies[i].transform.position = vectorPos;
        }
    }


    /// <summary>
    /// Mostra uma pequena explosão visual na posição dada.
    /// Destrói-se automaticamente após 0.1s para simular um flash.
    /// </summary>
    private void ShowExplosion(Vector3 pos)
    {
        var ex = Instantiate(explosionPrefab, pos, Quaternion.identity);
        Destroy(ex, 0.1f);
    }

    /// <summary>
    /// Remove o inimigo da cena e mostra explosão.
    /// </summary>
    public void HandleEnemyKilled(int enemy)
    {
        Vector3 vectorPos = enemies[enemy].transform.position;

        ShowExplosion(vectorPos);
        Destroy(enemies[enemy]);
        enemies.RemoveAt(enemy);
    }


    /// <summary>
    /// Atualiza o texto do score no ecrã.
    /// </summary>
    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    /// <summary>
    /// Remove todas as balas do jogador da cena.
    /// Usado quando se reinicia o jogo ou entre rondas.
    /// </summary>
    private void ClearPlayerBullets()
    {
        foreach (GameObject bullet in bullets)
            Destroy(bullet);
        bullets.Clear();
    }

    /// <summary>
    /// Lida com o fim do jogo. Limpa todos os objetos visuais e mostra o painel Game Over.
    /// </summary>
    private void GameOver()
    {
        ClearGame();
        ShowGameOver();
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Mostra uma mensagem de erro e volta ao menu principal.
    /// Usado quando o modelo deteta um erro crítico.
    /// </summary>
    public void DisplayError(string message)
    {
        if (errorLog != null)
        {
            errorLog.text = message;
            errorLog.gameObject.SetActive(true);
        }
        ClearGame();
        ShowMainMenu();
    }

    /// <summary>
    /// Limpa todos os objetos visuais
    /// </summary>
    private void ClearGame()
    {
        game = false;

        foreach (GameObject bullet in bullets)
            Destroy(bullet);
        bullets.Clear();

        foreach (GameObject enemy in enemies)
            Destroy(enemy);
        enemies.Clear();

        if (enemyBullet != null)
        {
            Destroy(enemyBullet);
            enemyBullet = null;
        }
    }

    /// <summary>
    /// Converte a estrutura Coord usada pelo modelo para o tipo Vector3 da Unity.
    /// </summary>
    private Vector3 CoordToVector(Coord coord)
    {
        return new Vector3(coord.x, coord.y, coord.z);
    }
}
