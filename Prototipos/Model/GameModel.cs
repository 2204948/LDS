using UnityEngine;
using System;
using System.Collections.Generic;

// Corrige para usar os tipos do Unity
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GameModel
{
    // Eventos
    public event Action<Vector3> OnPositionChanged;
    public event Action<Vector3> BulletFired;
    public event Action<Vector3> BulletMoved;
    public event Action<Vector3> BulletDestroyed;
    public event Action<Vector2> OnEnemySpawn;
    public event Action<Vector2> OnEnemyKilled;
    public event Action OnGameOver;

    // Estado do jogador
    private Vector3 playerPosition;

    // Estado de balas e inimigos
    private List<Vector3> bullets = new List<Vector3>();
    private List<Vector2> enemies = new List<Vector2>();

    // Outras variáveis
    public const float playerMovSpeed = 8.0f;
    public const float bulletSpeed = 10f;
    private readonly float maxLeft = -14f;
    private readonly float maxRight = 14f;
    private readonly float bulletMaxY = 20f;

    private int score = 0;

    // Métodos

    public void StartNewGame()
    {
        bullets.Clear();
        enemies.Clear();
        score = 0;
        playerPosition = new Vector3(0f, -14f, 0f);
        OnPositionChanged?.Invoke(playerPosition);
    }

    public void SetInitialPosition(Vector3 initialPosition)
    {
        playerPosition = initialPosition;
        OnPositionChanged?.Invoke(playerPosition);
    }

    public void Move(float direction, float deltaTime)
    {
        float deltaX = direction * playerMovSpeed * deltaTime;
        playerPosition.x = Mathf.Clamp(playerPosition.x + deltaX, maxLeft, maxRight);
        OnPositionChanged?.Invoke(playerPosition);
    }

    public void TryShot()
    {
        Vector3 bulletPos = playerPosition + new Vector3(0f, 1f, 0f);
        bullets.Add(bulletPos);
        BulletFired?.Invoke(bulletPos);
    }

    public void Update(float deltaTime)
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            bullets[i] += Vector3.up * bulletSpeed * deltaTime;

            if (bullets[i].y > bulletMaxY)
            {
                BulletDestroyed?.Invoke(bullets[i]);
                bullets.RemoveAt(i);
            }
            else
            {
                BulletMoved?.Invoke(bullets[i]);
            }
        }
    }

    public void EnemySpawn(Vector2 position)
    {
        enemies.Add(position);
        OnEnemySpawn?.Invoke(position);
    }

    public void EnemyHit(Vector2 position)
    {
        if (enemies.Remove(position))
        {
            OnEnemyKilled?.Invoke(position);
        }

        if (GameOverCheck())
        {
            OnGameOver?.Invoke();
        }
    }

    public bool GameOverCheck()
    {
        if (enemies.Count == 0)
        {
            return true;
        }

        foreach (var enemy in enemies)
        {
            if (enemy.y <= playerPosition.y)
            {
                return true;
            }
        }

        return false;
    }
}
