using Virtocommerce.MarketplaceReturn.Core.Models.Search;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.MarketplaceVendorModule.Core.Domains;
using VirtoCommerce.ReturnModule.Core.Models.Search;

namespace Virtocommerce.MarketplaceReturn.Data.Queries;

public class SearchReturnsQuery : SellerReturnSearchCriteria, IQuery<ReturnSearchResult>, IHasSellerId
{
    public string SellerName { get; set; }
}
