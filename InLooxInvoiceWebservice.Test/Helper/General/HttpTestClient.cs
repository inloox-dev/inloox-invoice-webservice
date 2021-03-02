using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace InLooxInvoiceWebservice.Test.Helper.General
{
    public class HttpTestClient
    {        
        public async Task<string> Get(string url)
        {
            using var client = new HttpClient();
            var resp = await client.GetAsync(url);
            return await resp.Content.ReadAsStringAsync();
        }

        public async Task<string> Post(string token, string view, object data)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(data);
            var dataRaw = new StringContent(json, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("origin", "https://www.inlooxnow.com");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync("https://app.inlooxnow.com/odata/" + view, dataRaw);
            var text = await response.Content.ReadAsStringAsync();
            return text;
        }
     }
}
