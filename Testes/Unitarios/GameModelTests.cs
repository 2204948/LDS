using NUnit.Framework;
using System.Collections.Generic;

public class GameModelTests
{

    // Define um campo privado ou readonly com um novo valor
    private void SetField<T>(object target, string fieldName, T value)
    {
        var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(field, $"Campo '{fieldName}' não encontrado em {target.GetType().Name}.");
        field.SetValue(target, value);
    }

    // Recupera o valor de um campo privado
    private T GetField<T>(object target, string fieldName)
    {
        var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(field, $"Campo '{fieldName}' não encontrado em {target.GetType().Name}.");
        return (T)field.GetValue(target);
    }

    [Test]
    public void StartNewGame_ShouldSetInitialState()
    {
        var model = new GameModel();
        Coord? position = null;
        List<Coord> enemies = null;
        int score = -1;

        model.OnPositionChanged += pos => position = pos;
        model.OnScoreChanged += s => score = s;
        model.OnEnemySpawn += e => enemies = e;

        model.StartNewGame();

        Assert.AreEqual(new Coord(0f, -13.5f, 0f), position);
        Assert.AreEqual(0, score);
        Assert.NotNull(enemies);
    }

    [Test]
    public void Move_Right_ShouldChangePlayerX()
    {
        var model = new GameModel();
        model.StartNewGame();

        Coord? lastPosition = null;
        model.OnPositionChanged += pos => lastPosition = pos;

        model.Move(1f, 1f); // deltaTime = 1 para simplificar

        Assert.NotNull(lastPosition);
        Assert.Greater(lastPosition.Value.x, 0f);
    }

    [Test]
    public void Move_Right_ShouldNotPassMaxX()
    {
        var model = new GameModel();
        model.StartNewGame();
        
        var playerPos = typeof(GameModel).GetField("playerPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        playerPos.SetValue(model, new Coord(13.9f, -13.5f, 0f));

        model.Move(1f, 0.1f);

        Coord newPlayerPos = GetField<Coord>(model, "playerPosition");
        Assert.Less(newPlayerPos.y, 14.001f);
    }

    [Test]
    public void TryShot_ShouldTriggerBulletFired()
    {
        var model = new GameModel();
        model.StartNewGame();

        Coord? bulletPosition = null;
        model.BulletFired += pos => bulletPosition = pos;

        model.TryShot();

        Assert.NotNull(bulletPosition);
        Assert.AreEqual(-12.5f, bulletPosition.Value.y); // -13.5 + 1
    }

    [Test]
    public void EnemyBulletHitPlayer_ShouldTriggerGameOver()
    {
        var model = new GameModel();
        bool gameOver = false;
        model.OnGameOver += () => gameOver = true;

        model.StartNewGame();

        // Forçar inimigo e tiro em cima do jogador
        var enemyBulletField = typeof(GameModel).GetField("enemyBullet", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        enemyBulletField.SetValue(model, new Coord(0f, -13.5f, 0f));

        model.OnUpdate(0.01f);

        Assert.IsTrue(gameOver, "O evento de Game Over não foi disparado.");
    }

    [Test]
    public void BulletHitsEnemy_ShouldTriggerScoreAndEnemyKilled()
    {
        var model = new GameModel();
        model.StartNewGame();

        bool enemyKilled = false;
        int score = -1;

        model.OnEnemyKilled += i => enemyKilled = true;
        model.OnScoreChanged += s => score = s;

        // Forçar inimigo numa posição conhecida
        var enemiesField = typeof(GameModel).GetField("enemies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        enemiesField.SetValue(model, new List<Coord> { new Coord(0f, 0f, 0f) });

        // Forçar bala na mesma posição
        var bulletsField = typeof(GameModel).GetField("bullets", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bulletsField.SetValue(model, new List<Coord> { new Coord(0f, 0f, 0f) });

        model.OnUpdate(0.01f);

        Assert.IsTrue(enemyKilled);
        Assert.AreEqual(10, score);
    }

    [Test]
    public void LastEnemyHit_ShouldTriggerClearPlayerBulletsAndIncreaseEnemySpeed()
    {
        var model = new GameModel();
        model.StartNewGame();

        bool clearBullets = false;
        model.ClearPlayerBullets += () => clearBullets = true;

        // Forçar inimigo numa posição conhecida
        var enemiesField = typeof(GameModel).GetField("enemies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        enemiesField.SetValue(model, new List<Coord> { new Coord(0f, 0f, 0f) });

        // Forçar bala na mesma posição
        var bulletsField = typeof(GameModel).GetField("bullets", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bulletsField.SetValue(model, new List<Coord> { new Coord(0f, 0f, 0f) });

        var speed = typeof(GameModel).GetField("enemySpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        speed.SetValue(model, 5f);

        model.OnUpdate(0.01f);

        float newSpeed = GetField<float>(model, "enemySpeed");

        Assert.IsTrue(clearBullets);
        Assert.GreaterOrEqual(newSpeed, 7f);
    }

    [Test]
    public void EnemyBullet_ShouldBeCreated_InCorrectPosition()
    {
        var model = new GameModel();
        model.StartNewGame();

        //var enemyPosition = new Coord(5f, 6f, 0f);
        SetField(model, "enemies", new List<Coord> { new Coord(5f, 6f, 0f) });

        Coord? firedBulletPosition = null;
        model.EnemyBulletFired += pos => firedBulletPosition = pos;

        model.OnUpdate(0f);

        Assert.NotNull(firedBulletPosition);
        Assert.AreEqual(5f, firedBulletPosition.Value.y);
    }

    [Test]
    public void Enemies_ShouldMoveAndInvertDirection()
    {
        var model = new GameModel();
        List<Coord> enemyMove = null;
        model.StartNewGame();

        // Inimigo suficientemente à direita para passar o limite após movimento
        model.EnemyMoved += pos => enemyMove = pos;
        SetField(model, "enemies", new List<Coord> { new Coord(13.9f, 5f, 0f) });
        SetField(model, "enemyMoveRight", true); // indo para a direita
        model.OnUpdate(0.1f); // aplica o movimento
        var updatedEnemies = GetField<List<Coord>>(model, "enemies");
        bool newDir = GetField<bool>(model, "enemyMoveRight");

        Assert.NotNull(enemyMove);
        Assert.IsFalse(newDir);
        Assert.AreEqual(4f, updatedEnemies[0].y, "O inimigo deveria ter descido uma unidade no eixo Y.");
    }

    [Test]
    public void Bullet_ShouldBeDestroyed_When_Y_GreaterThan_MaxY()
    {
        var model = new GameModel();
        model.StartNewGame();

        bool destroyed = false;
        model.BulletDestroyed += b => destroyed = true;

        SetField(model, "bullets", new List<Coord> { new Coord(0, 18.1f, 0) });

        model.OnUpdate(0f);

        var bullets = GetField<List<Coord>>(model, "bullets");

        Assert.IsTrue(destroyed, "BulletDestroyed não foi disparado.");
        Assert.AreEqual(0, bullets.Count, "A bala não foi removida.");
    }

    [Test]
    public void EnemyBullet_ShouldBeDestroyed_When_Y_LessThan_MinY()
    {
        var model = new GameModel();
        model.StartNewGame();

        bool destroyed = false;
        model.EnemyBulletDestroyed += () => destroyed = true;

        SetField(model, "enemyBullet", new Coord(0, -14.1f, 0));

        model.OnUpdate(0f);

        var bullets = GetField<Coord>(model, "enemyBullet");

        Assert.IsTrue(destroyed, "EnemyBulletDestroyed não foi disparado.");
    }

    [Test]
    public void Bullet_Move_ShouldChangeBulletY()
    {
        var model = new GameModel();
        model.StartNewGame();
        bool moved = false;
        model.BulletMoved += b => moved = true;

        SetField(model, "bullets", new List<Coord> { new Coord(0, -12f, 0) });

        model.OnUpdate(0.1f);

        var bullets = GetField<List<Coord>>(model, "bullets");

        Assert.IsTrue(moved, "BulletMoved não foi disparado.");
        Assert.Greater(bullets[0].y, -12f);
    }

    [Test]
    public void EnemyBullet_Move_ShouldChangeBulletY()
    {
        var model = new GameModel();
        model.StartNewGame();

        bool moved = false;
        model.EnemyBulletMoved += b => moved = true;

        SetField(model, "enemyBullet", new Coord(0, 5f, 0));

        model.OnUpdate(0.1f);

        var bullet = GetField<Coord>(model, "enemyBullet");

        Assert.IsTrue(moved, "BulletMoved não foi disparado.");
        Assert.Greater(5f, bullet.y);
    }
}