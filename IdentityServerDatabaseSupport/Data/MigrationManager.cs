
using DatabaseContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class MigrationManager
{
    public static async Task<IHost> MigrateDatabaseAsync(this IHost host)
    {
        await using (var scope = host.Services.CreateAsyncScope())
        {
            await using (var appContext = scope.ServiceProvider.GetRequiredService<DataContext>())
            {
                try
                {
                    await appContext.Database.MigrateAsync();
                }
                catch
                {
                    throw;
                }
            }
        }

        return host;
    }
}