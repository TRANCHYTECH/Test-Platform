namespace VietGeeks.TestPlatform.SharedKernel.Exceptions;

public class TestPlatformException : Exception
{
    public TestPlatformException()
    { 
    }

    public TestPlatformException(string message): base(message)
    {
    }

    public TestPlatformException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

