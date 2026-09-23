using System;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.MarketplaceReturn.Core.Models;
using VirtoCommerce.MarketplaceReturn.Core.Models.Search;
using VirtoCommerce.MarketplaceReturn.Core.Services;
using VirtoCommerce.MarketplaceReturn.Data.Models;
using VirtoCommerce.MarketplaceReturn.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.MarketplaceReturn.Data.Services;

public class SellerReturnSearchService : SearchService<SellerReturnSearchCriteria,
    SellerReturnSearchResult, SellerReturn, SellerReturnEntity>,
    ISellerReturnSearchService
{
    public SellerReturnSearchService(
        Func<ISellerReturnRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        ISellerReturnCrudService crudService,
        IOptions<CrudOptions> crudOptions
        )
        : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
    {
    }

    protected override IQueryable<SellerReturnEntity> BuildQuery(IRepository repository, SellerReturnSearchCriteria criteria)
    {
        var query = ((ISellerReturnRepository)repository).SellerReturns;

        if (!string.IsNullOrEmpty(criteria.SellerId))
        {
            query = query.Where(x => x.SellerId == criteria.SellerId);
        }

        //if (!criteria.Statuses.IsNullOrEmpty())
        //{
        //    query = query.Where(x => criteria.Statuses.Contains(x.Status));
        //}

        return query;
    }
}
