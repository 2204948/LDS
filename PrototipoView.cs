using UnityEngine;

public class GameView : MonoBehaviour
{
    private GameController controller;

    // Referências visuais definidas no Unity
    public GameObject player;
    public GameObject bulletPrefab;
    public GameObject enemyPrefab;
    public GameObject explosionPrefab;

    public GameObject mainMenuPanel; // Menu principal
    public GameObject gameOverPanel;
    public TMPro.TextMeshProUGUI scoreText;

    void Start()
    {
        controller = new GameController(this); // Liga a View ao Controller
        ShowMainMenu(); // Mostra o menu ao iniciar
    }

    void Update()
    {
        HandleUserInput(); // Lê input do jogador a cada frame
    }

    void HandleUserInput() // Lê entradas do teclado e envia para o Controller
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        bool shoot = Input.GetKeyDown(KeyCode.Space);
        controller.OnPlayerInput(horizontal, shoot);
    }

    public void ShowMainMenu() // Exibe o menu principal
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (player != null)
            player.SetActive(false); // Esconde jogador até começar
    }

    public void StartGame() // Inicia o jogo e esconde o menu
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (player != null)
            player.SetActive(true);

        UpdateScore(0); // Reinicia pontuação
    }

    public void UpdatePlayerPosition(Vector2 position) // Atualiza a posição da nave do jogador
    {
        player.transform.position = position;
    }

    public void SpawnBullet(Vector2 position) // Cria uma bala na cena
    {
        Instantiate(bulletPrefab, position, Quaternion.identity);
    }

    public void SpawnEnemy(Vector2 position) // Cria um inimigo na cena
    {
        Instantiate(enemyPrefab, position, Quaternion.identity);
    }

    public void ShowExplosion(Vector2 position) // Mostra uma explosão visual
    {
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        Destroy(explosion, 1.0f); // Destrói após 1 segundo
    }

    public void ShowGameOver() // Ativa o painel Game Over
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void UpdateScore(int score) // Atualiza o texto da pontuação
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }

    public void FlashPlayerDamage() // Faz o jogador piscar vermelho
    {
        StartCoroutine(FlashRed());
    }

    private System.Collections.IEnumerator FlashRed() // Coroutine para piscar vermelho
    {
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sr.color = original;
    }

    public void RemoveEnemy(GameObject enemy) // Remove inimigo da cena
    {
        Destroy(enemy);
    }
}
