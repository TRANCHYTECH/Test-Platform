using MongoDB.Entities;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.TestManager.Core.Models;

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

    protected override Action<T> OnBeforeSave<T>()
    {
        //var type = typeof(T);

        //if(type == typeof(MyTest))
        //{
        //    Action<MyTest> action = t =>
        //    {
        //        t.CreatedBy = _tenant.Email;
        //    };

        //    return action as Action<T>;
        //}

        return base.OnBeforeSave<T>();
    }
}