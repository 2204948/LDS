/// <summary>
/// Estrutura que representa uma posi��o (x, y, z) no espa�o 2D/3D do jogo.
/// Utilizada como base para todas as posi��es do modelo.
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
    /// Retorna um vetor na dire��o vertical positiva com dist�ncia dada.
    /// </summary>
    public static Coord Up(float distance)
    {
        return new Coord(0f, distance, 0f);
    }

    /// <summary>
    /// Retorna um vetor unit�rio para baixo.
    /// </summary>
    public static Coord Down()
    {
        return new Coord(0f, -1f, 0f);
    }

    /// <summary>
    /// Retorna um vetor na dire��o vertical negativa com dist�ncia dada.
    /// </summary>
    public static Coord Down(float distance)
    {
        return new Coord(0f, -distance, 0f);
    }

    /// <summary>
    /// Retorna um vetor unit�rio para a direita.
    /// </summary>
    public static Coord Right()
    {
        return new Coord(1f, 0f, 0f);
    }

    /// <summary>
    /// Retorna um vetor unit�rio para a esquerda.
    /// </summary>
    public static Coord Left()
    {
        return new Coord(-1f, 0f, 0f);
    }

    /// <summary>
    /// Limita um valor entre um m�nimo e um m�ximo.
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
    {
        return new Coord(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    /// <summary>
    /// Multiplica um vetor por um escalar.
    /// </summary>
    public static Coord operator *(Coord a, float scalar)
    {
        return new Coord(a.x * scalar, a.y * scalar, a.z * scalar);
    }
}
