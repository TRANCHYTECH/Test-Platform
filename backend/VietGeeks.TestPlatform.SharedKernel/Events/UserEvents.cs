namespace VietGeeks.TestPlatform.SharedKernel.Events
{
    public record UserCreatedEvent
    {
        public required string UserId { get; set; }
    }
}