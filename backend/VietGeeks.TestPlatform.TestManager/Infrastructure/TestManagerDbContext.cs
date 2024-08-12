using MongoDB.Entities;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public class TestManagerDbContext: DBContext
{
    private readonly ITenant _tenant;

    public TestManagerDbContext(ITenant tenant)
    {
        _tenant = tenant;
        ModifiedBy = new ModifiedBy
        {
            UserID = tenant.UserId,
            UserName = tenant.Email
        };

        SetGlobalFilterForBaseClass<EntityBase>(c => c.ModifiedBy.UserID == tenant.UserId);
    }
}