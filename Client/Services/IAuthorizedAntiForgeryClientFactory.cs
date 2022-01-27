using System.Net.Http;
using System.Threading.Tasks;

namespace Kvno.Bff.Client.Services
{
    public interface IAuthorizedAntiForgeryClientFactory
    {
        Task<HttpClient> CreateClient(string clientName = "authorizedClient");
    }
}