using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace PwdClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();

            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            var diso = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (diso.IsError)
            {
                Console.WriteLine(diso.Error);
                return;
            }

            var tokenClient = new TokenClient(diso.TokenEndpoint, "pwdClient", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("jesse", "123456");
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            else
            {
                Console.WriteLine(tokenResponse.Json);
            }

            var httpClient = new HttpClient();
            httpClient.SetBearerToken(tokenResponse.AccessToken);
            var response = await httpClient.GetAsync("http://localhost:5001/api/values");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }
    }
}