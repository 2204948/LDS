// # Responsavel por criar e ligar os componentes no inicio do jogo / Monta o MVC

using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private PlayerView playerView; // ligacao ao jogador

    private PlayerModel playerModel; 
    private PlayerController playerController;

    private void Start()
    {
        // Criar e ligar componentes
        playerModel = new PlayerModel();
        playerController = new PlayerController(playerModel);

        playerView.Subscribe(playerModel);
        playerView.SetController(playerController);

        playerModel.SetInitialPosition(new Vector3(0f, -14f, 0f));
    }
}
