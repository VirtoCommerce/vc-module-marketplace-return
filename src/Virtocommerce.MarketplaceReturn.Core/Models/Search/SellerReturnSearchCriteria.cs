using VirtoCommerce.ReturnModule.Core.Models.Search;

namespace Virtocommerce.MarketplaceReturn.Core.Models.Search;

public class SellerReturnSearchCriteria : ReturnSearchCriteria
{
    public string SellerId { get; set; }

    public string[] Statuses { get; set; }
}
