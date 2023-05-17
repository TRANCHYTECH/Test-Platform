using System;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.MapperProfiles
{
    public class EncryptIdConverter : ITypeConverter<string, EncryptedId>
    {
        private readonly IDataProtector _dataProtector;

        public EncryptIdConverter(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("EncryptedId");
        }

        public EncryptedId Convert(string source, EncryptedId destination, ResolutionContext context)
        {
            return source;
            //return _dataProtector.Protect(source);
        }
    }
}

