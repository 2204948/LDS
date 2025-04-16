using UnityEngine;

public class PlayerView : MonoBehaviour
{

    // Move o jogador de acordo com a direcao e velocidade recebidas
    public void Move(Vector2 direction, float speed)
    {
        // Aplicacao do movimento
        Vector3 movement = new Vector3(direction.x, 0f, 0f) * speed * Time.deltaTime;
        transform.position += movement; 
    }

    // Define diretamente a posicao do jogador 
    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}