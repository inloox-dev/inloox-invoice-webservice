using System.Threading.Tasks;

namespace InLooxInvoiceWebservice.Services
{
    public interface IService
    {
        Task<bool> Connect(string username, string password);
        Task<bool> Connect(string token);
    }
}