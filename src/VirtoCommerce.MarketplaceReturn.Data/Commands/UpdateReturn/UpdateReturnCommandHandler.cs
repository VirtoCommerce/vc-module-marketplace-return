using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceReturn.Core.Models;
using VirtoCommerce.MarketplaceReturn.Core.Services;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.ReturnModule.Core.Services;

namespace VirtoCommerce.MarketplaceReturn.Data.Commands;

public class UpdateReturnCommandHandler : ICommandHandler<UpdateReturnCommand>
{
    private readonly IReturnService _returnService;
    private readonly ISellerReturnCrudService _sellerReturnCrudService;
    private readonly ISellerReturnService _sellerReturnService;

    public UpdateReturnCommandHandler(
        IReturnService returnService,
        ISellerReturnCrudService sellerReturnCrudService,
        ISellerReturnService sellerReturnService
        )
    {
        _returnService = returnService;
        _sellerReturnCrudService = sellerReturnCrudService;
        _sellerReturnService = sellerReturnService;
    }

    public virtual async Task Handle(UpdateReturnCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.SellerId == null)
        {
            throw new ArgumentNullException(nameof(request.SellerId));
        }

        if (request.OrderReturn == null)
        {
            throw new ArgumentNullException(nameof(request.OrderReturn));
        }

        var orderReturn = request.OrderReturn;

        await _returnService.SaveChangesAsync([orderReturn]);

        var existedSellerReturn = await _sellerReturnService.GetSellerReturnById(request.SellerId, orderReturn.Id);

        if (existedSellerReturn == null)
        {
            existedSellerReturn = ExType<SellerReturn>.New();
            existedSellerReturn.SellerId = request.SellerId;
            existedSellerReturn.SellerName = request.SellerName;
            existedSellerReturn.ReturnId = orderReturn.Id;

            await _sellerReturnCrudService.SaveChangesAsync([existedSellerReturn]);
        }
    }
}
