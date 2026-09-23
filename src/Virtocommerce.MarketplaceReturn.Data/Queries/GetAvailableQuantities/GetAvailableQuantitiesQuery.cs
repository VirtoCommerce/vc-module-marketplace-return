using System.Collections.Generic;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;

namespace Virtocommerce.MarketplaceReturn.Data.Queries;

public class GetAvailableQuantitiesQuery : IQuery<Dictionary<string, int>>
{
    public string OrderId { get; set; }
}
