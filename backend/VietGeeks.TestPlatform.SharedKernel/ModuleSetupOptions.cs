namespace VietGeeks.TestPlatform.SharedKernel
{
    public class DatabaseOptions
    {
        public string DatabaseName { get; set; } = default!;

        public string ConnectionString { get; set; } = default!;
    }

    public class ServiceBusOptions
    {
        public string Namespace { get; set; } = default!;

        public string ManagedIdentityClientId { get; set; } = default!;
    }
}