namespace VietGeeks.TestPlatform.SharedKernel
{
    public class DatabaseOptions
    {
        public required string DatabaseName { get; init; }

        public required string ConnectionString { get; init; }
    }
}