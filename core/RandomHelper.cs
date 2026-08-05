namespace core;

public static class RandomHelper
{
    private static Random _random = new();

    public static int RandomInt(int max)
    {
        return _random.Next(max);
    }
}