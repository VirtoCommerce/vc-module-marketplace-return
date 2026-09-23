using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.MarketplaceReturn.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.MarketplaceReturn.Data.Models;

public class SellerReturnEntity : AuditableEntity, IDataEntity<SellerReturnEntity, SellerReturn>
{
    [Required]
    [StringLength(128)]
    public string ReturnId { get; set; }

    [StringLength(128)]
    public string ParentReturnId { get; set; }

    [StringLength(64)]
    public string SellerId { get; set; }

    [StringLength(255)]
    public string SellerName { get; set; }

    public virtual SellerReturn ToModel(SellerReturn model)
    {
        model.Id = Id;
        model.CreatedBy = CreatedBy;
        model.CreatedDate = CreatedDate;
        model.ModifiedBy = ModifiedBy;
        model.ModifiedDate = ModifiedDate;

        model.ReturnId = ReturnId;
        model.ParentReturnId = ParentReturnId;
        model.SellerId = SellerId;
        model.SellerName = SellerName;

        return model;
    }

    public virtual SellerReturnEntity FromModel(SellerReturn model, PrimaryKeyResolvingMap pkMap)
    {
        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedBy = model.CreatedBy;
        CreatedDate = model.CreatedDate;
        ModifiedBy = model.ModifiedBy;
        ModifiedDate = model.ModifiedDate;

        ReturnId = model.ReturnId;
        ParentReturnId = model.ParentReturnId;
        SellerId = model.SellerId;
        SellerName = model.SellerName;

        return this;
    }

    public virtual void Patch(SellerReturnEntity target)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        target.ReturnId = ReturnId;
        target.ParentReturnId = ParentReturnId;
        target.SellerId = SellerId;
        target.SellerName = SellerName;
    }
}
