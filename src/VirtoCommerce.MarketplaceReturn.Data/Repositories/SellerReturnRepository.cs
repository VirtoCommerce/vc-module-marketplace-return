using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.MarketplaceReturn.Data.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.MarketplaceReturn.Data.Repositories;

public class SellerReturnRepository : DbContextRepositoryBase<SellerReturnDbContext>, ISellerReturnRepository
{
    public SellerReturnRepository(SellerReturnDbContext dbContext)
        : base(dbContext)
    {
    }

    public IQueryable<SellerReturnEntity> SellerReturns => DbContext.Set<SellerReturnEntity>();

    public virtual async Task<IList<SellerReturnEntity>> GetSellerReturnByIdsAsync(IList<string> ids, string responseGroup = null)
    {
        var result = Array.Empty<SellerReturnEntity>();

        if (!ids.IsNullOrEmpty())
        {
            result = await SellerReturns.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }

        return result;
    }

}
