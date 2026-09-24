using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Security;
using MarketplaceReturnModule = VirtoCommerce.MarketplaceReturn.Core;
using VendorModule = VirtoCommerce.MarketplaceVendorModule.Core;

namespace VirtoCommerce.MarketplaceReturn.Web.Authorization;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseModuleAuthorization(this IApplicationBuilder appBuilder)
    {
        using var serviceScope = appBuilder.ApplicationServices.CreateScope();

        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        SavePredefinedRolesAsync(roleManager).GetAwaiter().GetResult();

        return appBuilder;
    }

    private static async Task SavePredefinedRolesAsync(RoleManager<Role> roleManager)
    {
        foreach (var vendorModuleRole in VendorModule.ModuleConstants.Security.Roles.AllRoles)
        {
            var existingVendorModuleRole = await roleManager.FindByIdAsync(vendorModuleRole.Id);
            var returnModuleRole = MarketplaceReturnModule.ModuleConstants.Security.Roles.AllRoles.FirstOrDefault(x => x.Id == vendorModuleRole.Id);

            if (existingVendorModuleRole != null)
            {
                vendorModuleRole.Permissions = existingVendorModuleRole.Permissions.Concat(vendorModuleRole.Permissions).Distinct().ToList();
                if (returnModuleRole != null)
                {
                    vendorModuleRole.Permissions = vendorModuleRole.Permissions.Concat(returnModuleRole.Permissions).Distinct().ToList();
                }
                await roleManager.UpdateAsync(vendorModuleRole);
            }
            else
            {
                if (returnModuleRole != null)
                {
                    vendorModuleRole.Permissions = vendorModuleRole.Permissions.Concat(returnModuleRole.Permissions).Distinct().ToList();
                }
                await roleManager.CreateAsync(vendorModuleRole);
            }
        }
    }
}
