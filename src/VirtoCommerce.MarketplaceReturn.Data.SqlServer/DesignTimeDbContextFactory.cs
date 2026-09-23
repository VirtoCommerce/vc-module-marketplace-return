using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.MarketplaceReturn.Data.Repositories;

namespace VirtoCommerce.MarketplaceReturn.Data.SqlServer;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SellerReturnDbContext>
{
    public SellerReturnDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SellerReturnDbContext>();
        var connectionString = args.Length != 0 ? args[0] : "Server=(local);User=virto;Password=virto;Database=VirtoCommerce3;";

        builder.UseSqlServer(
            connectionString,
            options => options.MigrationsAssembly(typeof(SqlServerDataAssemblyMarker).Assembly.GetName().Name));

        return new SellerReturnDbContext(builder.Options);
    }
}
