using System.Threading.Tasks;
using CarProduct.Client;
using Microsoft.AspNetCore.Mvc;

namespace CarProduct.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ICarsClient _carsClient;

        public TestController(ICarsClient carsClient)
        {
            _carsClient = carsClient;
        }

        [HttpGet]
        public async Task RunTest()
        {
            var cl = new CarsClient();

            await cl.GetProduct("vehicledetail/4758465e-ba52-422e-8ea4-cfef87b02445/");

            
        }
    }
}