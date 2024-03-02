using ECommerceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Services;

public static class DatabaseInitializerService
{
    public static void Initialize(ProductContext dbContext)
    {
        // Read the SQL scripts
        var script1 = File.ReadAllText("SqlScript.sql");
        var script2 = File.ReadAllText("SqlScript2.sql");

        using (var transaction = dbContext.Database.BeginTransaction())
        {
            try
            {
                // Execute the first script
                dbContext.Database.ExecuteSqlRaw(script1);

                // Commit the transaction for the first script
                transaction.Commit();

                // Begin a new transaction for the second script
                using (var transaction2 = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // Execute the second script
                        dbContext.Database.ExecuteSqlRaw(script2);

                        // Commit the transaction for the second script
                        transaction2.Commit();
                    }
                    catch (Exception)
                    {
                        // Rollback the transaction if an exception occurs during the second script execution
                        transaction2.Rollback();
                        throw; // Re-throw the exception
                    }
                }
            }
            catch (Exception)
            {
                // Rollback the transaction if an exception occurs during the first script execution
                transaction.Rollback();
                throw; // Re-throw the exception
            }
        }
    }
}