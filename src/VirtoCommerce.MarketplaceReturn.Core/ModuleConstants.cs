using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.MarketplaceReturn.Core;

public static class ModuleConstants
{
    public static class Security
    {
        public static class Roles
        {
            public static readonly Role Operator = new()
            {
                Id = "vcmp-operator-role",
                Permissions = new[]
                {
                    ReturnModule.Core.ModuleConstants.Security.Permissions.Access,
                    ReturnModule.Core.ModuleConstants.Security.Permissions.Read,
                    ReturnModule.Core.ModuleConstants.Security.Permissions.Update,
                }
                .Select(x => new Permission { GroupName = "Return", Name = x })
                .ToList()
            };

            public static readonly Role VendorOwner = new()
            {
                Id = "vcmp-owner-role",
                Permissions = new[]
                {
                    ReturnModule.Core.ModuleConstants.Security.Permissions.Access,
                    ReturnModule.Core.ModuleConstants.Security.Permissions.Read,
                    ReturnModule.Core.ModuleConstants.Security.Permissions.Update,
                }
                .Select(x => new Permission { GroupName = "Return", Name = x })
                .ToList()
            };

            public static readonly Role VendorAdmin = new()
            {
                Id = "vcmp-admin-role",
                Permissions = new[]
                {
                    ReturnModule.Core.ModuleConstants.Security.Permissions.Access,
                    ReturnModule.Core.ModuleConstants.Security.Permissions.Read,
                    ReturnModule.Core.ModuleConstants.Security.Permissions.Update,
                }
                .Select(x => new Permission { GroupName = "Return", Name = x })
                .ToList()
            };

            public static Role[] AllRoles = { Operator, VendorOwner, VendorAdmin };

        }
    }

    public static class Settings
    {
        public static class General
        {
            public static SettingDescriptor MarketplaceReturnEnabled { get; } = new()
            {
                Name = "Marketplacereturn.Enabled",
                GroupName = "MarketplaceReturn|General",
                ValueType = SettingValueType.Boolean,
                DefaultValue = false,
            };

            public static IEnumerable<SettingDescriptor> AllGeneralSettings
            {
                get
                {
                    yield return MarketplaceReturnEnabled;
                }
            }
        }

        public static IEnumerable<SettingDescriptor> AllSettings
        {
            get
            {
                return General.AllGeneralSettings;
            }
        }
    }

    public static readonly string DefaultSplitStatus = "Split";
}
