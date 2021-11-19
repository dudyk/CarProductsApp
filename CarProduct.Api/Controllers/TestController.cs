using CarProduct.Application.Settings;
using CarProduct.Client;
using CarProduct.Client.Models;
using CarProduct.Persistence.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CarProduct.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly CarsClientSettings _settings;

        public TestController(IOptions<CarsClientSettings> settings)
        {
            _settings = settings.Value;
        }

        [HttpGet]
        public async Task RunTest()
        {
            var cl = new CarsClient(_settings.Url, _settings.UserName, _settings.Password);

            await cl.GetProductsPage(new GetProductsPageRequest
            {
                StockType = StockTypes.Used.ToString(),
                Make = "tesla",
                Model = "tesla-model_s",
                Price = "100000",
                DistanceMiles = "all",
                Zip = "94596",
                PageCount = 1,
            });
        }
    }
}