using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Virtocommerce.MarketplaceReturn.Data.Models;
using VirtoCommerce.Platform.Core.Common;

namespace Virtocommerce.MarketplaceReturn.Data.Repositories;

public interface ISellerReturnRepository : IRepository
{
    IQueryable<SellerReturnEntity> SellerReturns { get; }

    Task<IList<SellerReturnEntity>> GetSellerReturnByIdsAsync(IList<string> ids, string responseGroup = null);
}
