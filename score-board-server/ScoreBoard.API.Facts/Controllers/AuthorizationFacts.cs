namespace ScoreBoard.API.Facts.Controllers
{
    public class AuthorizationFacts
    {
        // private readonly WebApplicationFactory<Startup> factory;

        //public AuthorizationFacts(WebApplicationFactory<Startup> factory)
        //{
        //    this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        //}

        //[Theory]
        //[InlineData("only-employees")]
        //[InlineData("only-managers")]
        //public async Task GivenUnauthorizedCall_WhenGetOnlyEmployees_ThenReturns403Forbidden(string action)
        //{
        //    HttpClient httpClient = factory.CreateClient();
        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"api/user/{action}");
        //    string apiKey = "{API Key}"; // Third party api key
        //    request.Headers.Add("X-Api-Key", apiKey);

        //    HttpResponseMessage response = await httpClient.SendAsync(request);
        //    string responseContent = await response.Content.ReadAsStringAsync();

        //    response.Content.Headers.ContentType.ToString().Should().Be("application/problem+json");
        //    responseContent.Should().Be("{\"type\":\"https://httpstatuses.com/403\",\"title\":\"Forbidden\",\"status\":403}"); // Really naive check, can't guarantee the order of the properties, but whatever :)
        //    response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        //}
    }
}
