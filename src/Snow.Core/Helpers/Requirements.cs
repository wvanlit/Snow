namespace Snow.Core.Helpers;

public static class Requirements
{
    public static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}
