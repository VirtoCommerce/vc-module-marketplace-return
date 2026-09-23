using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Virtocommerce.MarketplaceReturn.Data.Models;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace Virtocommerce.MarketplaceReturn.Data.Repositories;

public class SellerReturnDbContext : DbContextBase
{
    public SellerReturnDbContext(DbContextOptions<SellerReturnDbContext> options)
        : base(options)
    {
    }

    protected SellerReturnDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SellerReturnEntity>().ToTable("SellerReturn").HasKey(x => x.Id);
        modelBuilder.Entity<SellerReturnEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
        modelBuilder.Entity<SellerReturnEntity>().HasIndex(x => x.ReturnId).IsUnique();

        switch (Database.ProviderName)
        {
            case "Pomelo.EntityFrameworkCore.MySql":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("Virtocommerce.MarketplaceReturn.Data.MySql"));
                break;
            case "Npgsql.EntityFrameworkCore.PostgreSQL":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("Virtocommerce.MarketplaceReturn.Data.PostgreSql"));
                break;
            case "Microsoft.EntityFrameworkCore.SqlServer":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("Virtocommerce.MarketplaceReturn.Data.SqlServer"));
                break;
        }
    }
}
