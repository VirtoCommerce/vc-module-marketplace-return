using VirtoCommerce.MarketplaceReturn.Core.Models;
using VirtoCommerce.MarketplaceReturn.Core.Models.Search;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.MarketplaceReturn.Core.Services;

public interface ISellerReturnSearchService : ISearchService<SellerReturnSearchCriteria, SellerReturnSearchResult, SellerReturn>
{
}
