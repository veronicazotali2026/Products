using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.Extensions;
using Entities.Responses;
using Microsoft.AspNetCore.Mvc;
using Products.Presentation.APIS;
using Shared.Response;

namespace Products.Presentation.Applications;

public class CommandHandler : ApiControllerBase
{
    protected async Task<IActionResult> ExecuteCreateCommandAsync<TCommand,TEntity>(Func<Guid,TCommand,Task<ApiBaseResponse>> func, TCommand cmd, Guid id, string route, CancellationToken cancellationToken) 
        where TEntity : BaseResponse
    {
        var result = await func(id, cmd);
        var response = result.GetResult<TEntity>();
        return CreatedAtRoute(route, new { Id = response.Id }, response);
    }
    
    protected async Task<IActionResult> ExecuteCreateCommandAsync<TCommand,TEntity>(Func<TCommand,Task<ApiBaseResponse>> func, TCommand cmd, string route, CancellationToken cancellationToken) 
        where TEntity : BaseResponse
    {
        var result = await func(cmd);
        var response = result.GetResult<TEntity>();
        return CreatedAtRoute(route, new { Id = response.Id }, response);
    }
    
    protected async Task<IActionResult> ExecuteUpdateCommandAsync<TCommand>(Func<Guid,TCommand,Task> func, TCommand cmd, Guid id, CancellationToken cancellationToken) 
    {
        await func(id,cmd);
        return NoContent();
    }
    
    protected async Task<IActionResult> ExecuteDeleteCommandAsync(Func<Guid,Guid,Task> func, Guid parentId, Guid childId, CancellationToken cancellationToken)
    {
        await func(parentId,childId);
        return NoContent();
    }
    
    protected async Task<IActionResult> ExecuteUpdateCommandAsync<TCommand>(Func<TCommand,Task> func, TCommand cmd, CancellationToken cancellationToken) 
    {
        await func(cmd);
        return NoContent();
    }
    
    protected async Task<IActionResult> ExecuteDeleteCommandAsync(Func<Guid,Task> func, Guid parentId, CancellationToken cancellationToken)
    {
        await func(parentId);
        return NoContent();
    }
}