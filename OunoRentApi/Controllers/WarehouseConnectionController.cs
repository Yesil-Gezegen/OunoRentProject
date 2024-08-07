using BusinessLayer.CQRS.WarehouseConnection.Command;
using BusinessLayer.CQRS.WarehouseConnection.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.WarehouseConnection.Request;

namespace OunoRentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseConnectionController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehouseConnectionController(IMediator mediator)
    {
        _mediator = mediator;
    }
  
}