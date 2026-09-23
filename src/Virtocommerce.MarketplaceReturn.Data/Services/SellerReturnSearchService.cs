using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Virtocommerce.MarketplaceReturn.Core.Models;
using Virtocommerce.MarketplaceReturn.Core.Models.Search;
using Virtocommerce.MarketplaceReturn.Core.Services;
using Virtocommerce.MarketplaceReturn.Data.Models;
using Virtocommerce.MarketplaceReturn.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace Virtocommerce.MarketplaceReturn.Data.Services;

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
