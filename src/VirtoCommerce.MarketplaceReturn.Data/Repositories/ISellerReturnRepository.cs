using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceReturn.Data.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketplaceReturn.Data.Repositories;

public interface ISellerReturnRepository : IRepository
{
    IQueryable<SellerReturnEntity> SellerReturns { get; }

    Task<IList<SellerReturnEntity>> GetSellerReturnByIdsAsync(IList<string> ids, string responseGroup = null);
}
