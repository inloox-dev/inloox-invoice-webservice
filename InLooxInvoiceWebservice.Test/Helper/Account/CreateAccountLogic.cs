using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InLooxInvoiceWebservice.Test.Helper.Account
{
    public class CreateAccountLogic
    {
        public async Task<string> GetApprovedAccountToken(CreateAccountModel createAccount) {
            // gets short living access url
            var response = await CreateAccount(createAccount);

            // activate account, needs some time to activate
            await Task.Delay(3000);
            var tokenUrl = await ApproveAccount(response.ret.url);
            await Task.Delay(3000);

            // gets long living access token
            var tokenUri = new Uri(tokenUrl);
            var token = System.Web.HttpUtility.ParseQueryString(tokenUri.Fragment.Substring(1)).Get("access_token");

            return token;
        }

        public async Task<CreateAccountResponse> CreateAccount(CreateAccountModel createAccount)
        {
            using var client = new HttpClient();

            var json = JsonConvert.SerializeObject(createAccount);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("origin", "https://www.inlooxnow.com");
            var response = await client.PostAsync("https://app.inlooxnow.com/AzureAccount/CreateAndGetLoginToken", data);

            if (response.IsSuccessStatusCode)
            {
                var text = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CreateAccountResponse>(text);
            }

            return null;
        }

        public async Task<string> ApproveAccount(string url)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookies
            };

            using var client = new HttpClient(handler);
            var response = await client.GetAsync(url);

            var welcomePage = await response.Content.ReadAsStringAsync();
            var reg = new Regex(@"<input\s+name=""__RequestVerificationToken""\s+.*?\s+value=""(.*?)""");
            var g = reg.Match(welcomePage).Groups[1].Value;

            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("AccountName", "TestTestTest"),
                new KeyValuePair<string, string>("ApprovedTermsOfService", "true"),
                new KeyValuePair<string, string>("__RequestVerificationToken", g)
            };

            var data = new FormUrlEncodedContent(keyValues);
            var resp = await client.PostAsync("https://app.inlooxnow.com/Account/Welcome", data);

            return await GetLoginToken(cookies);
        }

        async Task<string> GetLoginToken(CookieContainer cookies)
        {
            var handler = new HttpClientHandler { AllowAutoRedirect = false, CookieContainer = cookies };

            using var client = new HttpClient(handler);

            var response = await client.GetAsync(
                "https://app.inlooxnow.com/oauth2/authorize"
                    + "?client_id=F1CE1597340F423E90F52FAE6438F0E4&redirect_uri="
                    + "https%3A%2F%2Fapp.inlooxnow.com%2Ftests%2Foauthsignin&state="
                    + "my-nonce&scope=Projects%20Plannings&response_type=token");

            var location = response.Headers.Location;
            return location.ToString();
        }
    }
}
