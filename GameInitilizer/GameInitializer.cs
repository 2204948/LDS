// # Responsavel por criar e ligar os componentes no inicio do jogo / Monta o MVC

using UnityEngine;

public class GameInitilizer : MonoBehaviour
{
    [SerializeField] private GameView view; 

    private GameModel model;
    private GameController controller;

    private void Start()
    {
        // Criar e ligar componentes
        model = new GameModel();
        controller = new GameController(model);

        view.Subscribe(model);
        view.SetController(controller);

        model.SetInitialPosition(new Vector3(0f, -14f, 0f));
    }
}
