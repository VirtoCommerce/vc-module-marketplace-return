using System;
using VirtoCommerce.MarketplaceVendorModule.Core.Domains;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ReturnModule.Core.Models;

namespace VirtoCommerce.MarketplaceReturn.Core.Models;

public class SellerReturn : AuditableEntity, IHasSellerId, ICloneable
{
    public string SellerId { get; set; }

    public string SellerName { get; set; }

    public string ReturnId { get; set; }

    public string ParentReturnId { get; set; }

    public Return Return { get; set; }

    public virtual object Clone()
    {
        return MemberwiseClone();
    }
}
