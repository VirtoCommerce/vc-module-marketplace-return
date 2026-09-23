using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Virtocommerce.MarketplaceReturn.Core.Models;
using Virtocommerce.MarketplaceReturn.Core.Services;
using Virtocommerce.MarketplaceReturn.Data.Models;
using Virtocommerce.MarketplaceReturn.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace Virtocommerce.MarketplaceReturn.Data.Services;

public class SellerReturnCrudService : CrudService<SellerReturn, SellerReturnEntity,
    GenericChangedEntryEvent<SellerReturn>, GenericChangedEntryEvent<SellerReturn>>,
    ISellerReturnCrudService
{
    public SellerReturnCrudService(
        Func<ISellerReturnRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        IEventPublisher eventPublisher
        ) : base(repositoryFactory, platformMemoryCache, eventPublisher)
    {
    }

    protected override Task<IList<SellerReturnEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
    {
        return ((ISellerReturnRepository)repository).GetSellerReturnByIdsAsync(ids, responseGroup);
    }
}
