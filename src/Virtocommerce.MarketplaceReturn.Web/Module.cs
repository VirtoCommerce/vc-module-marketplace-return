using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.MarketplaceReturn.Core;
using VirtoCommerce.MarketplaceReturn.Core.Services;
using VirtoCommerce.MarketplaceReturn.Data.Handlers;
using VirtoCommerce.MarketplaceReturn.Data.MySql;
using VirtoCommerce.MarketplaceReturn.Data.PostgreSql;
using VirtoCommerce.MarketplaceReturn.Data.Repositories;
using VirtoCommerce.MarketplaceReturn.Data.Services;
using VirtoCommerce.MarketplaceReturn.Data.SqlServer;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.MySql.Extensions;
using VirtoCommerce.Platform.Data.PostgreSql.Extensions;
using VirtoCommerce.Platform.Data.SqlServer.Extensions;
using VirtoCommerce.ReturnModule.Core.Events;

namespace VirtoCommerce.MarketplaceReturn.Web;

public class Module : IModule, IHasConfiguration
{
    public ManifestModuleInfo ModuleInfo { get; set; }
    public IConfiguration Configuration { get; set; }

    public void Initialize(IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<SellerReturnDbContext>(options =>
        {
            var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
            var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

            switch (databaseProvider)
            {
                case "MySql":
                    options.UseMySqlDatabase(connectionString, typeof(MySqlDataAssemblyMarker), Configuration);
                    break;
                case "PostgreSql":
                    options.UsePostgreSqlDatabase(connectionString, typeof(PostgreSqlDataAssemblyMarker), Configuration);
                    break;
                default:
                    options.UseSqlServerDatabase(connectionString, typeof(SqlServerDataAssemblyMarker), Configuration);
                    break;
            }
        });

        serviceCollection.AddTransient<ISellerReturnRepository, SellerReturnRepository>();
        serviceCollection.AddTransient<Func<ISellerReturnRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ISellerReturnRepository>());

        serviceCollection.AddTransient<ISellerReturnService, SellerReturnService>();
        serviceCollection.AddTransient<ISellerReturnCrudService, SellerReturnCrudService>();
        serviceCollection.AddTransient<ISellerReturnSearchService, SellerReturnSearchService>();

        serviceCollection.AddTransient<IReturnSplitter, SellerReturnSplitter>();

        serviceCollection.AddTransient<ReturnCreatedEventHandler>();

        serviceCollection.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<SellerReturnDbContext>());
    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
        var serviceProvider = appBuilder.ApplicationServices;

        // Register settings
        var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
        settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

        appBuilder.RegisterEventHandler<ReturnChangedEvent, ReturnCreatedEventHandler>();

        // Register permissions
        var permissionsRegistrar = serviceProvider.GetRequiredService<IPermissionsRegistrar>();
        permissionsRegistrar.RegisterPermissions(ModuleInfo.Id, "MarketplaceReturn", ModuleConstants.Security.Permissions.AllPermissions);

        // Apply migrations (creates this module's own VcmpReturn table; the base Return table is untouched)
        using var serviceScope = serviceProvider.CreateScope();
        using var dbContext = serviceScope.ServiceProvider.GetRequiredService<SellerReturnDbContext>();
        dbContext.Database.Migrate();
    }

    public void Uninstall()
    {
        // Nothing to do here
    }
}
