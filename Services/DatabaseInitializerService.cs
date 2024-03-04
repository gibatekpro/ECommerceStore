using ECommerceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Services;

public static class DatabaseInitializerService
    {
        public static void Initialize(ProductContext dbContext)
        {
            var script1 = DbType.IsSqlLite ? File.ReadAllText("SqlScriptLite.sql") : File.ReadAllText("SqlScript.sql");
            var script2 = DbType.IsSqlLite ? File.ReadAllText("SqlScriptLite2.sql") : File.ReadAllText("SqlScript2.sql");

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    
                    dbContext.Database.ExecuteSqlRaw(script1);
                    
                    dbContext.Database.ExecuteSqlRaw(script2);
                    
                    transaction.Commit();
                }
                catch (Exception)
                {
                    
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }