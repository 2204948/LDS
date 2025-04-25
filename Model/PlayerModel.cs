// # Responsavel por guardar o estado do jogador e aplica as regars, ex velocidade/ limites
// # NEmite evento da nova posicao para a view
using UnityEngine;
using System;

public class PlayerModel
{   
    public event Action<Vector3> OnPositionChanged;

    private Vector3 position;  // não pode ser Vector2 vendo que estamos a usar apenas 2D?
    public const float movSpeed = 8.0f;
    private readonly float maxLeft = -14f; 
    private readonly float maxRight = 14f; 

    public void SetInitialPosition(Vector3 initialPosition)
    {
        position = initialPosition;
        OnPositionChanged?.Invoke(position);
    }
    
    public void Move(float direction, float deltaTime)
    {
        float deltaX = direction * movSpeed * deltaTime ;
        position.x = Mathf.Clamp(position.x +  deltaX, maxLeft, maxRight);

        OnPositionChanged?.Invoke(position);
    }
}
