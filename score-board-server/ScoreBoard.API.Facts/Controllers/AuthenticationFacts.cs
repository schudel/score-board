using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ScoreBoard.API.Facts.Controllers
{
    public class AuthenticationFacts : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public AuthenticationFacts(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        [Theory]
        [InlineData("chat")]
        [InlineData("game")]
        [InlineData("livematch")]
        [InlineData("match")]
        [InlineData("player")]
        [InlineData("rating")]
        [InlineData("team")]
        public async Task GivenUnauthenticatedCall_WhenGetProtectedEndpoint_ThenReturns401Unauthorized(string action)
        {
            HttpClient httpClient = factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"api/{action}");

            HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);

            response.ReasonPhrase.Should().Be("Unauthorized");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        //[Fact]
        //public async Task GivenUnauthenticatedCall_WhenWrongCredentials_ThenReturns401Unauthorized()
        //{
        //    AuthenticationDto authenticationDto = new AuthenticationDto
        //    {
        //        PlayerName = FakeData.AdminName, 
        //        Password = "ff"
        //    };

        //    HttpClient httpClient = factory.CreateClient();
        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/authentication/authenticate");
        //    request.Content = new StringContent(JsonConvert.SerializeObject(authenticationDto), Encoding.UTF8, "application/json");

        //    HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);

        //    response.ReasonPhrase.Should().Be("Unauthorized");
        //    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        //}
    }
}
