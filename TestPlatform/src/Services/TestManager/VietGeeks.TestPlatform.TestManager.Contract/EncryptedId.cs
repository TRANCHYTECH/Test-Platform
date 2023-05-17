using System;
namespace VietGeeks.TestPlatform.TestManager.Contract
{
    public class EncryptedId
    {
        public string Value { get; private set; }


        public EncryptedId(string value)
        {
            Value = value;
        }

        public static implicit operator EncryptedId(string value) => new EncryptedId(value);
    }
}

