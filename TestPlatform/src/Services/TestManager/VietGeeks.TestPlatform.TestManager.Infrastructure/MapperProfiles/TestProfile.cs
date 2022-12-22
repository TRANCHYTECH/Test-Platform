using System;
using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.MappingProfiles
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<NewTestViewModel, MyTest>();
            CreateMap<MyTest, TestViewModel>();
        }
    }
}

