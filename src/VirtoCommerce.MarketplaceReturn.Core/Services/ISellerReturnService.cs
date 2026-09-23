using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceReturn.Core.Models;

namespace VirtoCommerce.MarketplaceReturn.Core.Services;

public interface ISellerReturnService
{
    Task<string> GetSellerIdAsync(string returnId);

    Task<IList<string>> GetReturnIdsForSellerAsync(string sellerId);

    Task<SellerReturn> GetSellerReturnById(string sellerId, string retrnId);

}
