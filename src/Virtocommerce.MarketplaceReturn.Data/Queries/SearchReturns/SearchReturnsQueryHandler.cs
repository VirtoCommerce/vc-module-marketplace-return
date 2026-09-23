using System;
using System.Threading;
using System.Threading.Tasks;
using Virtocommerce.MarketplaceReturn.Core.Services;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.ReturnModule.Core.Models.Search;
using VirtoCommerce.ReturnModule.Core.Services;

namespace Virtocommerce.MarketplaceReturn.Data.Queries;

public class SearchReturnsQueryHandler : IQueryHandler<SearchReturnsQuery, ReturnSearchResult>
{
    private readonly IReturnSearchService _returnSearchService;
    private readonly ISellerReturnService _sellerReturnService;

    public SearchReturnsQueryHandler(
        IReturnSearchService returnSearchService,
        ISellerReturnService sellerReturnService
        )
    {
        _returnSearchService = returnSearchService;
        _sellerReturnService = sellerReturnService;
    }

    public virtual async Task<ReturnSearchResult> Handle(SearchReturnsQuery request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (!string.IsNullOrEmpty(request.SellerId))
        {
            var returnIds = await _sellerReturnService.GetReturnIdsForSellerAsync(request.SellerId);
            if (returnIds.Count == 0)
            {
                return new ReturnSearchResult();
            }

            request.ObjectIds = returnIds;
        }

        var result = await _returnSearchService.SearchAsync(request);

        return result;
    }
}
