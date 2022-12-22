using MongoDB.Entities;
using VietGeeks.TestPlatform.AspNetCore;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure
{
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
}