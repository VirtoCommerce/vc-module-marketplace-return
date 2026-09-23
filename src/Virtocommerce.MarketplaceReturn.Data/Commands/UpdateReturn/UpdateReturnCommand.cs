using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.MarketplaceVendorModule.Core.Domains;
using VirtoCommerce.ReturnModule.Core.Models;

namespace Virtocommerce.MarketplaceReturn.Data.Commands;

public class UpdateReturnCommand : ICommand, IHasSellerId
{
    public string SellerId { get; set; }

    public string SellerName { get; set; }

    public Return OrderReturn { get; set; }
}
