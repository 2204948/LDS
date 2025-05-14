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

    public static Coord Up(float distance)
    {
        return new Coord(0f,distance, 0f);
    }

    public static Coord Down()
    {
        return new Coord(0f, -1f, 0f);
    }

    public static Coord Down(float distance)
    {
        return new Coord(0f, -distance, 0f);
    }

    public static Coord Right()
    {
        return new Coord(1f, 0f, 0f);
    }

    public static Coord Left()
    {
        return new Coord(-1f, 0f, 0f);
    }
    public static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static Coord operator +(Coord a, Coord b)
    {
        return new Coord(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Coord operator *(Coord a, float scalar)
    {
        return new Coord(a.x * scalar, a.y * scalar, a.z * scalar);
    }
}
