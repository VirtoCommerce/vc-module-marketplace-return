using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.MarketplaceReturn.Core;

public static class ModuleConstants
{
    public static class Security
    {
        public static class Permissions
        {
            public const string Access = "marketplace-return:access";
            public const string Create = "marketplace-return:create";
            public const string Read = "marketplace-return:read";
            public const string Update = "marketplace-return:update";
            public const string Delete = "marketplace-return:delete";

            public static string[] AllPermissions { get; } =
            [
                Access,
                Create,
                Read,
                Update,
                Delete,
            ];
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
