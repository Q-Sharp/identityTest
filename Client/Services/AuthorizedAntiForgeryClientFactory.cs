using Microsoft.JSInterop;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kvno.Bff.Client.Services
{
    public class AuthorizedAntiForgeryClientFactory : IAuthorizedAntiForgeryClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJSRuntime _jSRuntime;

        public AuthorizedAntiForgeryClientFactory(IHttpClientFactory httpClientFactory, IJSRuntime jSRuntime)
        {
            _httpClientFactory = httpClientFactory;
            _jSRuntime = jSRuntime;
        }

        public async Task<HttpClient> CreateClient(string clientName = "authorizedClient")
        {
            var token = await _jSRuntime.InvokeAsync<string>("getAntiForgeryToken");

            var client = _httpClientFactory.CreateClient(clientName);
            client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", token);

            return client;
        }
    }
}
