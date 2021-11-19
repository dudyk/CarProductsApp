using AutoMapper;
using CarProduct.Api.Models;
using CarProduct.Application.Commands;
using CarProduct.Application.Queries;
using CarProduct.Persistence.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarProduct.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ProductController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            return await _mediator.Send(new GetAllProductsQuery());
        }

        [HttpGet("{vehicleId}")]
        public async Task<Product> Get(string vehicleId)
        {
            if (string.IsNullOrWhiteSpace(vehicleId))
                throw new BadHttpRequestException("VehicleId couldn't be empty");

            return await _mediator.Send(new GetProductByVehicleIdQuery(vehicleId));
        }

        [HttpPost]
        public async Task Post(CreateProductRequest createProductRequest)
        {
            var command = _mapper.Map<CreateProductsCommand>(createProductRequest);
            await _mediator.Send(command);
        }

        [HttpDelete("{vehicleId}")]
        public async Task Delete(string vehicleId)
        {
            if (string.IsNullOrWhiteSpace(vehicleId))
                throw new BadHttpRequestException("VehicleId couldn't be empty");

            await _mediator.Send(new DeleteProductByVehicleIdCommand(vehicleId));
        }
    }
}