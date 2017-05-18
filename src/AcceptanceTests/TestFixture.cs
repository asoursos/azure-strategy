using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AcceptanceTests.Dsl;
using EasyTrade.App;
using EasyTrade.App.RequestHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AcceptanceTests
{
    public class TestFixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly TestServer _server;

        public TestFixture()
        {
            //var format = new Mock<ISecureDataFormat<AuthenticationTicket>>(MockBehavior.Strict);

            //format.Setup(mock => mock.Unprotect(It.Is<string>(token => token == "invalid-token")))
            //    .Returns(value: null);

            //format.Setup(mock => mock.Unprotect(It.Is<string>(token => token == "valid-token")))
            //    .Returns(delegate {
            //        var identity = new ClaimsIdentity(OAuthValidationDefaults.AuthenticationScheme);
            //        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "Fabrikam"));

            //        var properties = new AuthenticationProperties();

            //        return new AuthenticationTicket(new ClaimsPrincipal(identity),
            //            properties, OAuthValidationDefaults.AuthenticationScheme);
            //    });
            
            var builder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseStartup<TStartup>()
                .UseEnvironment("Test");

            _server = new TestServer(builder);

            var client = _server.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5000");
            ApiDriver= new ApiDriver(client);
        }

        public IApiDriver ApiDriver { get; }

        public void Dispose()
        {
            ApiDriver.Dispose();
            _server.Dispose();
        }
    }

    internal static class HttpClientHelper
    {
        public static async Task<TResult> PostAsync<TResult>(this HttpClient client, string relativeUrl, object data) //where TResult:object
        {
            var response= InvokeApi(client, relativeUrl, HttpMethod.Post, data).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(content);
        }

        private static async Task<HttpResponseMessage> InvokeApi(this HttpClient client, string relativeUrl,
            HttpMethod method, object data = null)
        {
            try
            {
                // Arrange
                var request = new HttpRequestMessage(method, relativeUrl);
                if (data != null )
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8,
                        "application/json");
                }

                return await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}