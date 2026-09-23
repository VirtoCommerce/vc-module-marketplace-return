using System.Threading.Tasks;
using Virtocommerce.MarketplaceReturn.Core.Models;
using VirtoCommerce.ReturnModule.Core.Models;

namespace Virtocommerce.MarketplaceReturn.Core.Services;

public interface IReturnSplitter
{
    Task<SellerReturn[]> SplitReturn(Return orderReturn);
}
