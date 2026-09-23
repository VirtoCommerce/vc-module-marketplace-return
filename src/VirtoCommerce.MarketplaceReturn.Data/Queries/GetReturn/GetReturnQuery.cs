using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.MarketplaceVendorModule.Core.Domains;
using VirtoCommerce.ReturnModule.Core.Models;

namespace VirtoCommerce.MarketplaceReturn.Data.Queries;

public class GetReturnQuery : IQuery<Return>, IHasSellerId
{
    public string Id { get; set; }

    public string SellerId { get; set; }
    public string SellerName { get; set; }
}
