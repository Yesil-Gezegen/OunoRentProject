using System.Net;
using System.Net.Http.Json;
using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared.DTO.Slider.Request;
using Shared.DTO.Slider.Response;

namespace OunoRentApiTests;

public class SliderControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SliderControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region CreateSlider
    [Fact]
    public async Task CreateSlider_ReturnsSuccessAsync()
    {
        //Arrange
        var faker = new Faker<CreateSliderRequest>()
            .CustomInstantiator(f => new CreateSliderRequest(
                Title: f.Lorem.Text(),
                MainImageUrl: f.Image.PlaceImgUrl(),
                MobileImageUrl: f.Image.PlaceImgUrl(),
                TargetUrl: f.Internet.DomainName(),
                Duration: f.Random.Int(1, 10),
                OrderNumber: f.Random.Int(1, 10000),
                ActiveFrom: f.Date.Past().ToUniversalTime(),
                ActiveTo: f.Date.Future().ToUniversalTime(),
                IsActive: f.Random.Bool(),
                MainImage: null,
                MobileImage:null
            ));

        var fakeSlider = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("/api/slider/", fakeSlider);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<SliderResponse>();

        //Assert
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.SliderId);
    }

    [Fact]
    public async Task CreateSlider_ReturnsBadRequestAsync()
    {
        //Arrange
        var faker = new Faker<CreateSliderRequest>()
            .CustomInstantiator(f => new CreateSliderRequest(
                Title: f.Random.Bool(1) ? null : f.Lorem.Text(),
                MainImageUrl: f.Image.PlaceImgUrl(),
                MobileImageUrl: f.Image.PlaceImgUrl(),
                TargetUrl: f.Internet.DomainName(),
                Duration: f.Random.Int(1, 10),
                OrderNumber: f.Random.Int(1, 10000),
                ActiveFrom: f.Date.Past(),
                ActiveTo: f.Date.Future(),
                IsActive: f.Random.Bool(),
                MainImage: null,
                MobileImage:null
            ));

        var fakeSlider = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("/api/slider", fakeSlider);
        var content = await response.Content.ReadFromJsonAsync<SliderResponse>();

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(Guid.Empty, content.SliderId);
    }
    #endregion

    #region GetSliders
    [Fact]
    public async Task GetSliders_ReturnSuccessAsync()
    {
        //Act
        var response = await _client.GetAsync("/api/slider");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<List<GetSlidersResponse>>();

        //Assert
        Assert.NotNull(content);
    }
    
    [Fact]
    public async Task GetActiveSliders_ReturnSuccessAsync()
    {
        //Act
        var response = await _client.GetAsync("/api/slider/GetActive");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<List<GetSlidersResponse>>();

        //Assert
        Assert.NotNull(content);
        foreach (var item in content)
        {
            Assert.True(item.IsActive);
        }
    }
    #endregion

    #region GetSlider
    [Fact]
    public async Task GetSlider_ReturnsSuccessAsync()
    {
        //Arrange
        var getAllresponse = await _client.GetAsync("/api/slider");
        getAllresponse.EnsureSuccessStatusCode();
        var sliderList = await getAllresponse.Content.ReadFromJsonAsync<List<GetSlidersResponse>>();

        var slider = sliderList.FirstOrDefault();

        // Act
        var response = await _client.GetAsync($"/api/slider/{slider.SliderId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<GetSliderResponse>();

        // Act
        Assert.NotNull(sliderList);
        Assert.NotNull(content);
        Assert.Equal(slider.SliderId, content.SliderId);
    }
    #endregion

    #region UpdateSlider
    [Fact]
    public async Task UpdateSlider_ReturnsSuccessAsync()
    {
        //Arrange
        var getAllresponse = await _client.GetAsync("/api/slider");
        getAllresponse.EnsureSuccessStatusCode();
        var sliderList = await getAllresponse.Content.ReadFromJsonAsync<List<GetSlidersResponse>>();

        var slider = sliderList.FirstOrDefault();

        //Act
        var faker = new Faker<UpdateSliderRequest>()
            .CustomInstantiator(f => new UpdateSliderRequest(
                SliderId: slider.SliderId,
                Title: f.Lorem.Text(),
                MainImageUrl: f.Image.PlaceImgUrl(),
                MobileImageUrl: f.Image.PlaceImgUrl(),
                TargetUrl: f.Internet.DomainName(),
                Duration: f.Random.Int(1, 10),
                OrderNumber: f.Random.Int(1, 10000),
                ActiveFrom: f.Date.Past().ToUniversalTime(),
                ActiveTo: f.Date.Future().ToUniversalTime(),
                IsActive: f.Random.Bool(),
                MainImage: null,
                MobileImage: null
            ));

        var fakeSlider = faker.Generate();

        var response = await _client.PutAsJsonAsync($"/api/slider/{slider.SliderId}", fakeSlider);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<SliderResponse>();

        var updatedResponse = await _client.GetAsync($"/api/slider/{fakeSlider.SliderId}");
        updatedResponse.EnsureSuccessStatusCode();
        var updatedContent = await updatedResponse.Content.ReadFromJsonAsync<GetSliderResponse>();

        //Assert
        Assert.NotNull(sliderList);
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.SliderId);
        Assert.Equal(slider.SliderId, content.SliderId);
        Assert.NotNull(updatedContent);
        Assert.Equal(content.SliderId, updatedContent.SliderId);
        Assert.NotEqual(slider.Title, updatedContent.Title);
    }

    [Fact]
    public async Task UpdateSlider_ReturnsBadRequestAsync()
    {
        //Arrange
        var getAllresponse = await _client.GetAsync("/api/slider");
        getAllresponse.EnsureSuccessStatusCode();
        var sliderList = await getAllresponse.Content.ReadFromJsonAsync<List<GetSlidersResponse>>();

        var slider = sliderList.FirstOrDefault();

        //Act
        var faker = new Faker<UpdateSliderRequest>()
            .CustomInstantiator(f => new UpdateSliderRequest(
                SliderId: slider.SliderId,
                Title: f.Random.Bool(1) ? null : f.Lorem.Text(),
                MainImageUrl: f.Image.PlaceImgUrl(),
                MobileImageUrl: f.Image.PlaceImgUrl(),
                TargetUrl: f.Internet.DomainName(),
                Duration: f.Random.Int(1, 10),
                OrderNumber: f.Random.Int(1, 10000),
                ActiveFrom: f.Date.Past().ToUniversalTime(),
                ActiveTo: f.Date.Future().ToUniversalTime(),
                IsActive: f.Random.Bool(),
                MainImage: null,
                MobileImage: null
            ));

        var fakeSlider = faker.Generate();

        var response = await _client.PutAsJsonAsync($"/api/slider/{slider.SliderId}", fakeSlider);
        var content = await response.Content.ReadFromJsonAsync<SliderResponse>();

        var updatedResponse = await _client.GetAsync($"/api/slider/{fakeSlider.SliderId}");
        updatedResponse.EnsureSuccessStatusCode();
        var updatedContent = await updatedResponse.Content.ReadFromJsonAsync<GetSliderResponse>();

        //Assert
        Assert.NotNull(sliderList);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(Guid.Empty, content.SliderId);
        Assert.NotNull(updatedContent);
        Assert.Equal(slider.Title, updatedContent.Title);
    }
    #endregion

    #region DeleteSlider
    [Fact]
    public async Task DeleteSlider_ReturnsSuccessAsync()
    {
        // Arrange
        var getAllresponse = await _client.GetAsync("/api/slider");
        var sliderList = await getAllresponse.Content.ReadFromJsonAsync<List<GetSlidersResponse>>();

        var slider = sliderList.FirstOrDefault();

        // Act
        var response = await _client.DeleteAsync($"/api/slider/{slider.SliderId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<Guid>();

        var updatedResponse = await _client.GetAsync($"/api/slider/{slider.SliderId}");
        var updatedContent = await updatedResponse.Content.ReadFromJsonAsync<GetSliderResponse>();

        // Assert
        Assert.NotNull(content);
        Assert.Equal(content, slider.SliderId);
        Assert.Equal(HttpStatusCode.NotFound, updatedResponse.StatusCode);
        Assert.Equal(Guid.Empty, updatedContent.SliderId);
    }

    [Fact]
    public async Task DeleteSlider_ReturnsNotFoundAsync()
    {
        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/slider/49e7697d-7d11-404d-af72-beed870e301c");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }
    #endregion
}
