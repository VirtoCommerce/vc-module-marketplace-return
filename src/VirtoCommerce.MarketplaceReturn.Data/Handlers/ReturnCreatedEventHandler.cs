using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceReturn.Core;
using VirtoCommerce.MarketplaceReturn.Core.Models;
using VirtoCommerce.MarketplaceReturn.Core.Services;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.ReturnModule.Core.Events;
using VirtoCommerce.ReturnModule.Core.Services;

namespace VirtoCommerce.MarketplaceReturn.Data.Handlers;

public class ReturnCreatedEventHandler : IEventHandler<ReturnChangedEvent>
{
    private readonly ICustomerOrderService _customerOrderService;
    private readonly IReturnSplitter _returnSplitter;
    private readonly ISellerReturnCrudService _sellerReturnCrudService;
    private readonly IReturnService _returnService;


    public ReturnCreatedEventHandler(
        ICustomerOrderService customerOrderService,
        IReturnSplitter returnSplitter,
        ISellerReturnCrudService sellerReturnCrudService,
        IReturnService returnService
        )
    {
        _customerOrderService = customerOrderService;
        _returnSplitter = returnSplitter;
        _sellerReturnCrudService = sellerReturnCrudService;
        _returnService = returnService;
    }
    public virtual async Task Handle(ReturnChangedEvent message)
    {
        var addedReturns = message.ChangedEntries.Where(x => x.EntryState == EntryState.Added).Select(e => e.NewEntry).ToArray();

        foreach (var addedReturn in addedReturns)
        {
            var order = addedReturn.Order;
            if (order == null)
            {
                order = await _customerOrderService.GetByIdAsync(addedReturn.OrderId);
                addedReturn.Order = order;
            }

            if (order != null)
            {
                if (string.IsNullOrEmpty(order.ParentOperationId))
                {
                    var splitReturns = await _returnSplitter.SplitReturn(addedReturn);
                    addedReturn.Status = ModuleConstants.DefaultSplitStatus;

                    foreach (var splitReturn in splitReturns)
                    {
                        using (EventSuppressor.SuppressEvents())
                        {
                            await _returnService.SaveChangesAsync([splitReturn.Return]);
                        }
                        splitReturn.ReturnId = splitReturn.Return.Id;
                        await _sellerReturnCrudService.SaveChangesAsync([splitReturn]);
                    }

                    await _returnService.SaveChangesAsync([addedReturn]);
                }
                else
                {
                    var sellerReturn = ExType<SellerReturn>.New();
                    sellerReturn.Return = addedReturn;
                    sellerReturn.ReturnId = addedReturn.Id;
                    sellerReturn.ParentReturnId = null;
                    sellerReturn.SellerId = order.EmployeeId;

                    await _sellerReturnCrudService.SaveChangesAsync([sellerReturn]);
                }
            }
        }


    }
}
