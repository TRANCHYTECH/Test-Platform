namespace VietGeeks.TestPlatform.SharedKernel;

public static class EnumExtensions
{
    public static string Value(this Enum value)
    {
        return value.ToString("d");
    }

    public static string Key(this Enum value)
    {
        return value.ToString("G");
    }
}
