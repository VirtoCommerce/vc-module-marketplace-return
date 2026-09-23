using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceReturn.Core.Services;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ReturnModule.Core.Models;
using VirtoCommerce.ReturnModule.Core.Services;

namespace VirtoCommerce.MarketplaceReturn.Data.Queries;

public class GetReturnQueryHandler : IQueryHandler<GetReturnQuery, Return>
{
    private readonly IReturnService _returnService;
    private readonly ISellerReturnService _sellerReturnService;

    public GetReturnQueryHandler(
        IReturnService returnService,
        ISellerReturnService sellerReturnService
        )
    {
        _returnService = returnService;
        _sellerReturnService = sellerReturnService;
    }

    public virtual async Task<Return> Handle(GetReturnQuery request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var orderReturn = await _returnService.GetByIdAsync(request.Id);

        if (orderReturn != null && !string.IsNullOrEmpty(request.SellerId))
        {
            var ownerSellerId = await _sellerReturnService.GetSellerIdAsync(orderReturn.Id);
            if (ownerSellerId != request.SellerId)
            {
                return null;
            }
        }

        return orderReturn;
    }
}
