// # Lida com o input recebido do view e atualiza o Model
using UnityEngine;

public class PlayerController
{
    private PlayerModel model; 
    
    public PlayerController(PlayerModel model) 
    { 
        this.model = model; 
    }

    public void HandleInput(float direction, float deltaTime)
    {
        model.Move(direction, deltaTime);
    }
}
