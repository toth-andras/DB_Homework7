using Homework7.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace Homework7.DiExtensions;

public static class EFExtensions
{
    public static IServiceCollection AddEf(this IServiceCollection serviceCollection)
    {
        
        serviceCollection.AddDbContext<DatabaseContext>(
            options => options.UseNpgsql("name=ConnectionStrings:DefaultConnection"));

        return serviceCollection;
    }
}