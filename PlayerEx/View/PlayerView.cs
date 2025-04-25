// # Responsavel por representar o jogador visualmente, capta input e reage ao Model
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private PlayerController controller;

    // Liga a View ao Controller
    public void SetController(PlayerController controller)
    {
        this.controller = controller;
    }

    // Liga a View ao Model (para escutar eventos)
    public void Subscribe(PlayerModel model)
    {
        model.OnPositionChanged += UpdateVisual;
    }

    private void UpdateVisual(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    // Capta o input do jogador e envia ao Controller
    private void Update()
    {
        float inputMov = Input.GetAxisRaw("Horizontal");
        if (inputMov != 0f)
            controller?.playerMove(inputMov, Time.deltaTime);
        
    }
}
