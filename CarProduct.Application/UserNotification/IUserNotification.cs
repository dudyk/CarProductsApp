using System.Threading.Tasks;
using CarProduct.Persistence.Models;

namespace CarProduct.Application.UserNotification
{
    public interface IUserNotification
    {
        Task NotifyCreateProduct(Product product);
    }
}