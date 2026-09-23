using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.ReturnModule.Core.Services;

namespace Virtocommerce.MarketplaceReturn.Data.Queries.GetAvailableQuantities;

public class GetAvailableQuantitiesQueryHandler : IQueryHandler<GetAvailableQuantitiesQuery, Dictionary<string, int>>
{
    private readonly IReturnService _returnService;

    public GetAvailableQuantitiesQueryHandler(
        IReturnService returnService
        )
    {
        _returnService = returnService;
    }

    public virtual async Task<Dictionary<string, int>> Handle(GetAvailableQuantitiesQuery request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.OrderId == null)
        {
            throw new ArgumentNullException(nameof(request.OrderId));
        }

        var result = await _returnService.GetItemsAvailableQuantities(request.OrderId);

        return result;
    }
}
