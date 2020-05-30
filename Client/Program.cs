using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// 寫法一
        /// </summary>
        /// <returns></returns>
        //private static async Task MainAsync()
        //{
        //    // Discover endpoinets from metadata
        //    var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
        //    if(disco.IsError)
        //    {
        //        Console.WriteLine(disco.Error);
        //        return;
        //    }


        //    // 打到 IdentityServer的 TokenEndpoint 網址, 給 Client ID 和 Client Secret 以訪問資源
        //    var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");

        //    // 訪問到資源後,給 Scope 以取得 Access Token
        //    var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
        //    if(tokenResponse.IsError)
        //    {
        //        Console.WriteLine(tokenResponse.Error);
        //        return;
        //    }


        //    // 印出 Access Token
        //    Console.WriteLine($"Token: {tokenResponse.Json}\r\n");


        //    // 取得 Token 後, 就能訪問 Resource Server 資源 (Call API)
        //    var client = new HttpClient();
        //    client.SetBearerToken(tokenResponse.AccessToken);

        //    var response = await client.GetAsync("http://localhost:5010/identity");
        //    if(!response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine(response.StatusCode);
        //    }
        //    else
        //    {
        //        var content = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine(JArray.Parse(content));
        //    }
        //}


        /// <summary>
        /// 寫法二
        /// </summary>
        /// <returns></returns>
        private static async Task MainAsync()
        {
            var client = new HttpClient();

            // discover endpoints from metadata
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenResponse = await client.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = IdentityModel.OidcConstants.GrantTypes.ClientCredentials,

                ClientId = "client",
                ClientSecret = "secret",

                Parameters =
                    {
                        { "scope", "api1" }
                    }
            });

            Console.WriteLine("Token:");
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5010/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}
