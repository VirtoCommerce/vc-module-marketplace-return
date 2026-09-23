using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Virtocommerce.MarketplaceReturn.Core.Models;
using Virtocommerce.MarketplaceReturn.Core.Services;
using Virtocommerce.MarketplaceReturn.Data.Repositories;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;

namespace Virtocommerce.MarketplaceReturn.Data.Services;

public class SellerReturnService : ISellerReturnService
{
    private readonly Func<ISellerReturnRepository> _repositoryFactory;

    public SellerReturnService(Func<ISellerReturnRepository> repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }

    public async Task<string> GetSellerIdAsync(string returnId)
    {
        using var repository = _repositoryFactory();
        var entity = await repository.SellerReturns.FirstOrDefaultAsync(x => x.ReturnId == returnId);
        return entity?.SellerId;
    }

    public async Task<IList<string>> GetReturnIdsForSellerAsync(string sellerId)
    {
        using var repository = _repositoryFactory();
        return await repository.SellerReturns
            .Where(x => x.SellerId == sellerId)
            .Select(x => x.ReturnId)
            .ToListAsync();
    }

    public async Task<SellerReturn> GetSellerReturnById(string sellerId, string returnId)
    {
        using var repository = _repositoryFactory();
        var result = (await repository.SellerReturns.FirstOrDefaultAsync(x => x.SellerId == sellerId && x.ReturnId == returnId))?.ToModel(ExType<SellerReturn>.New());

        return result;
    }

}
