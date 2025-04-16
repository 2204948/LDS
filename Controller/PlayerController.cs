using UnityEngine;

public class PlayerController : MonoBehaviour // Herda MonoBehaviour para que o script possa ser anexado a um GameObject (neste caso, o jogador)
{
    private PlayerModel model; // Guarda uma instancia do PlayerModel
    private PlayerView view; // Guarda uma instancia de PlayerView

    private void Awake() // Metodo chamado automaticamente pelo Unity quando o GameObject e inicializado
    {
        model = new PlayerModel(); // Cria instancia do PlayerModel
        view = GetComponent<PlayerView>(); // Obtem o componente PlayerView que esta no mesmo GameObject
    }


    // Chamado pelo unity a cada frame
    void Update()
    {
        // Le o input horizontal do jogador (A ou seta para a esquerda = -1, D ou seta para a direita = 1)
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
        view.Move(input, model.GetMoveSpeed()); // Resposta a view, dependendo dos valores de speed estabelecidos por model
        
        Vector3 clamped = model.ClampPosition(view.transform.position); // verificacao de posicoes maximas
        view.SetPosition(clamped); // Caso ultrapasse os limites, a posicao e corrigida
    }
}
