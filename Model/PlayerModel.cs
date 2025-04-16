using UnityEngine;

public class PlayerModel
{   
    // Constante de velocidade de moviemnto
    public const float MovSpeed = 6.0f;
    // getter da velocidade de movimento
    public float GetMoveSpeed() => MovSpeed;

    // Limite de movimentos horizontais
    private readonly float maxLeft = -14f; // Limite esquerdo
    private readonly float maxRight = 14f; // Limite direito




    // // Recebe uma posicao e devolve uma nova com o eixo X limitado, entre os valores definidos
    public Vector3 ClampPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, maxLeft, maxRight), // restricao dos movimentos (clamp)
            position.y, // Fixo, sendo que apenas move na horizontal
            position.z // Fixo, e irrelevante mas necessario para o unity
            );            
    }
}
