using IdentityModel.Client;
using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace DotNetCoreConsoleApp
{
    // Illustrates client credentials flow to get access to an api
    class Program
    {
        private const string authority = "https://localhost:5101";
        private const string scopes = "demoapi-read";
        private const string api = "https://localhost:5105";
        private const string clientId = "dotnetcorebackendservice";
        private const string clientSecret = "put dev secret in user secrets file";

        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            DiscoveryResponse authorityMetadata;
            try
            {
                authorityMetadata = await DiscoveryClient.GetAsync(authority);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
            if(authorityMetadata.IsError)
            {
                Console.WriteLine(authorityMetadata.Error);
                return;
            }

            var tokenFetcher = new TokenClient(authorityMetadata.TokenEndpoint, clientId, clientSecret);
            var response = await tokenFetcher.RequestClientCredentialsAsync(scopes);
            if(response.IsError)
            {
                Console.WriteLine(response.Error);
                Console.ReadKey();
                return;
            }
            Console.WriteLine(response.Json);


            string responseText;
            try
            {
                responseText = await api
                    .AppendPathSegments("api", "values")
                    .WithOAuthBearerToken(response.AccessToken)
                    .GetStringAsync();
            }
            catch (FlurlHttpException ex)
            {
                Console.WriteLine(ex);
                return;
            }

            Console.WriteLine("Response: ", responseText);
            Console.ReadKey();
        }
    }
}
