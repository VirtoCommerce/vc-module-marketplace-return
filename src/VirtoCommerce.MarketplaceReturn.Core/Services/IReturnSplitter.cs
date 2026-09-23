using System.Threading.Tasks;
using VirtoCommerce.MarketplaceReturn.Core.Models;
using VirtoCommerce.ReturnModule.Core.Models;

namespace VirtoCommerce.MarketplaceReturn.Core.Services;

public interface IReturnSplitter
{
    Task<SellerReturn[]> SplitReturn(Return orderReturn);
}
