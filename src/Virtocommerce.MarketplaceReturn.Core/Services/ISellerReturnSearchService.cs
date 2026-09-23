using Virtocommerce.MarketplaceReturn.Core.Models;
using Virtocommerce.MarketplaceReturn.Core.Models.Search;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace Virtocommerce.MarketplaceReturn.Core.Services;

public interface ISellerReturnSearchService : ISearchService<SellerReturnSearchCriteria, SellerReturnSearchResult, SellerReturn>
{
}
