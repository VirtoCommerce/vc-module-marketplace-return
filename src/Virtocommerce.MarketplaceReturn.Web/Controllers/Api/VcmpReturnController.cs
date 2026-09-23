using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.MarketplaceReturn.Data.Commands;
using VirtoCommerce.MarketplaceReturn.Data.Queries;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.MarketplaceVendorModule.Data.Authorization;
using VirtoCommerce.ReturnModule.Core.Models;
using VirtoCommerce.ReturnModule.Core.Models.Search;
using Permissions = VirtoCommerce.MarketplaceReturn.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.MarketplaceReturn.Web.Controllers.Api;

[Authorize]
[Route("api/vcmp/return")]
public class VcmpReturnController : Controller
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationService _authorizationService;

    public VcmpReturnController(
        IMediator mediator,
        IAuthorizationService authorizationService
        )
    {
        _mediator = mediator;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Search returns that belong to the current seller
    /// </summary>
    [HttpPost]
    [Route("search")]
    public async Task<ActionResult<ReturnSearchResult>> SearchReturns([FromBody] SearchReturnsQuery query)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, query, new SellerAuthorizationRequirement(Permissions.Read));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Find a return by id, scoped to the current seller
    /// </summary>
    [HttpGet]
    [Route("getbyid")]
    public async Task<ActionResult<Return>> GetReturnById([FromQuery] string id)
    {
        var query = ExType<GetReturnQuery>.New();
        query.Id = id;

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, query, new SellerAuthorizationRequirement(Permissions.Read));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Create or update a return; the seller is always taken from the current user, never from the request body
    /// </summary>
    [HttpPost]
    [Route("update")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateReturn([FromBody] UpdateReturnCommand command)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, command, new SellerAuthorizationRequirement(Permissions.Update));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Available item quantities for the order with the passed id
    /// </summary>
    [HttpGet]
    [Route("available-quantities/{orderId}")]
    [Authorize(Permissions.Read)]
    public async Task<ActionResult<Dictionary<string, int>>> GetAvailableQuantities(string orderId)
    {
        var query = ExType<GetAvailableQuantitiesQuery>.New();
        query.OrderId = orderId;

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, query, new SellerAuthorizationRequirement(Permissions.Read));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
