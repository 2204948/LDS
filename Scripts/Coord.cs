/// <summary>
/// Estrutura que representa uma posição (x, y, z) no espaço 2D/3D do jogo.
/// Utilizada como base para todas as posições do modelo.
/// </summary>
public struct Coord
{
    public float x;
    public float y;
    public float z;

    public Coord(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Retorna um vetor na direção vertical positiva com distância dada.
    /// </summary>
    public static Coord Up(float distance)
        => new Coord(0f, distance, 0f);

    /// <summary>
    /// Retorna um vetor unitário para baixo.
    /// </summary>
    public static Coord Down()
        => new Coord(0f, -1f, 0f);

    /// <summary>
    /// Retorna um vetor na direção vertical negativa com distância dada.
    /// </summary>
    public static Coord Down(float distance)
        => new Coord(0f, -distance, 0f);

    /// <summary>
    /// Retorna um vetor unitário para a direita.
    /// </summary>
    public static Coord Right()
        => new Coord(1f, 0f, 0f);

    /// <summary>
    /// Retorna um vetor unitário para a esquerda.
    /// </summary>
    public static Coord Left()
        => new Coord(-1f, 0f, 0f);

    /// <summary>
    /// Limita um valor entre um mínimo e um máximo.
    /// </summary>
    public static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary>
    /// Soma dois vetores coordenados.
    /// </summary>
    public static Coord operator +(Coord a, Coord b)
        => new Coord(a.x + b.x, a.y + b.y, a.z + b.z);

    /// <summary>
    /// Multiplica um vetor por um escalar.
    /// </summary>
    public static Coord operator *(Coord a, float scalar)
        => new Coord(a.x * scalar, a.y * scalar, a.z * scalar);
}
