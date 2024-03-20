using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection("Share collection")]
public class AuctionControllerTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly Guid sampleAuctionId = Guid.Parse("afbee524-5972-4075-8800-7d1f9d7b0a0c");

    public AuctionControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAuctions_ShouldReturn3Auctions()
    {
        // arrange

        // act
        var response = await _httpClient.GetFromJsonAsync<List<AuctionDto>>("api/auctions");

        // assert
        Assert.Equal(10, response.Count);
    }

    [Fact]
    public async Task GetAuctionsById_WithValidId_ShouldReturnAuction()
    {
        // arrange

        // act
        var response = await _httpClient.GetFromJsonAsync<AuctionDto>($"api/auctions/{sampleAuctionId}");

        // assert
        Assert.NotNull(response);
        Assert.Equal(sampleAuctionId, response.Id);
    }

    [Fact]
    public async Task GetAuctionsById_WithInValidId_ShouldReturnNotFound()
    {
        // arrange

        // act
        var response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAuctionsById_WithInValidId_ShouldReturnBadRequest()
    {
        // arrange

        // act
        var response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid}");

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuction_WithNoAuth_ShouldReturn401()
    {
        // arrange
        var auction = new AuctionDto() { Make = "test" };

        // act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuction_WithAuth_ShouldReturn201()
    {
        // arrange
        var auction = GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.getBearerForUser("bob"));

        // act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
        Assert.Equal("bob", createdAuction.Seller);
    }

    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
    {
        // arrange
        var auction = GetAuctionForCreate();
        auction.Make = null;
        _httpClient.SetFakeJwtBearerToken(AuthHelper.getBearerForUser("bob"));

        // act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
    {
        // arrange
        var auction = GetAuctionForCreate();
        auction.Make = "MakeNewTest";
        _httpClient.SetFakeJwtBearerToken(AuthHelper.getBearerForUser("bob"));

        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{sampleAuctionId}", auction);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
        // arrange
        var auction = GetAuctionForCreate();
        auction.Make = "MakeNewTest";
        _httpClient.SetFakeJwtBearerToken(AuthHelper.getBearerForUser("alice"));

        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{sampleAuctionId}", auction);

        // assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelper.ReInitDbForTests(db);
        return Task.CompletedTask;
    }

    public CreateAuctionDto GetAuctionForCreate()
    {
        return new CreateAuctionDto()
        {
            Make = "MakeTest",
            Model = "ModelTest",
            ImageUrl = "ImageUrlTest",
            Color = "ColorTest",
            Mileage = 99,
            Year = 99,
            ReservePrice = 99
        };
    }
}