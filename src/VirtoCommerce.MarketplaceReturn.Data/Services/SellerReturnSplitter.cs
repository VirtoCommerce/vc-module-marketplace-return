using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceReturn.Core.Models;
using VirtoCommerce.MarketplaceReturn.Core.Services;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ReturnModule.Core.Models;
using VirtoCommerce.StoreModule.Core.Services;
using ReturnModuleConstants = VirtoCommerce.ReturnModule.Core.ModuleConstants;

namespace VirtoCommerce.MarketplaceReturn.Data.Services;

public class SellerReturnSplitter : IReturnSplitter
{
    private readonly ICustomerOrderSearchService _customerOrderSearchService;
    private readonly IStoreService _storeService;
    private readonly IUniqueNumberGenerator _uniqueNumberGenerator;
    private readonly ISettingsManager _settingsManager;

    public SellerReturnSplitter(
        ICustomerOrderSearchService customerOrderSearchService,
        IStoreService storeService,
        IUniqueNumberGenerator uniqueNumberGenerator,
        ISettingsManager settingsManager
        )
    {
        _customerOrderSearchService = customerOrderSearchService;
        _storeService = storeService;
        _uniqueNumberGenerator = uniqueNumberGenerator;
        _settingsManager = settingsManager;
    }
    public virtual async Task<SellerReturn[]> SplitReturn(Return orderReturn)
    {
        var result = new List<SellerReturn>();

        var settingDescriptor = ReturnModuleConstants.Settings.General.ReturnNewNumberTemplate;
        var numberTemplate = await _settingsManager.GetValueAsync<string>(settingDescriptor);
        var storeId = orderReturn.Order?.StoreId;
        if (!string.IsNullOrEmpty(storeId))
        {
            var store = await _storeService.GetByIdAsync(storeId);
            numberTemplate = store.Settings.GetValue<string>(settingDescriptor);
        }

        var childOrdersSearchCriteria = ExType<CustomerOrderSearchCriteria>.New();
        childOrdersSearchCriteria.ParentOperationId = orderReturn.OrderId;

        var childOrders = (await _customerOrderSearchService.SearchAsync(childOrdersSearchCriteria)).Results.ToList();
        var childOrderLineItemsDict = childOrders
            .SelectMany(x => x.Items.Select(y => y.ProductId))
            .Distinct()
            .ToDictionary(key => key, value => childOrders.FirstOrDefault(y => y.Items.Select(z => z.ProductId).ToList().Contains(value))?.Id);

        var dividedByOrderReturnLineItemsDict = new Dictionary<string, List<ReturnLineItem>>();

        foreach (var returnLineItem in orderReturn.LineItems)
        {
            var productInParentOrderId = orderReturn.Order?.Items.FirstOrDefault(x => x.Id == returnLineItem.OrderLineItemId)?.ProductId;
            if (!string.IsNullOrEmpty(productInParentOrderId) && childOrderLineItemsDict.TryGetValue(productInParentOrderId, out var inOrderId))
            {
                if (!dividedByOrderReturnLineItemsDict.ContainsKey(inOrderId))
                {
                    dividedByOrderReturnLineItemsDict[inOrderId] = [];
                }
                dividedByOrderReturnLineItemsDict[inOrderId].Add(returnLineItem);
            }
        }

        foreach (var keyValue in dividedByOrderReturnLineItemsDict)
        {
            var childOrder = childOrders.FirstOrDefault(x => x.Id == keyValue.Key);

            var splitReturn = ExType<Return>.New();
            splitReturn.Number = _uniqueNumberGenerator.GenerateNumber(numberTemplate);
            splitReturn.LineItems = [];
            foreach (var splitReturnLineItem in keyValue.Value)
            {
                var newSplitItem = (ReturnLineItem)splitReturnLineItem.Clone();
                newSplitItem.Id = null;
                newSplitItem.OrderLineItemId = childOrder.Items.FirstOrDefault(x => x.ProductId == orderReturn.Order?.Items.FirstOrDefault(x => x.Id == splitReturnLineItem.OrderLineItemId)?.ProductId).Id;

                splitReturn.LineItems.Add(newSplitItem);
            }
            splitReturn.OrderId = keyValue.Key;
            splitReturn.Order = childOrder;
            splitReturn.Status = orderReturn.Status;

            var sellerReturn = ExType<SellerReturn>.New();
            sellerReturn.Return = splitReturn;
            sellerReturn.ParentReturnId = orderReturn.Id;
            sellerReturn.SellerId = childOrder.EmployeeId;

            result.Add(sellerReturn);
        }

        return result.ToArray();
    }
}
