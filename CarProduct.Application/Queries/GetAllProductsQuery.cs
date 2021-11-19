using System.Collections.Generic;
using MediatR;
using CarProduct.Persistence.Models;

namespace CarProduct.Application.Queries
{
    public class GetAllProductsQuery: IRequest<IList<Product>>
    {
    }
}
