using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CSharpAPITemplate.Data;

/// <summary>
/// Factory helps to create migrations in design-time.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public ApplicationDbContext CreateDbContext(string[] args)
    {
	    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
	    // TODO: Get connection string from configuration.
        optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=api-example;User Id=postgres;Password=postgres;");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}