using System.Net;
using System.Net.Http.Headers;
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
    // Arrange
    var faker = new Faker<CreateSliderRequest>()
        .CustomInstantiator(f => new CreateSliderRequest(
            Title: f.Lorem.Sentence(),
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

    // Gerçek dosya yolları
    var mainImagePath = "/Users/ozgurgokmen/source/repos/OunoRentProject/OunoRentApiTests/TestAssets/Cat03.jpg";
    var mobileImagePath = "/Users/ozgurgokmen/source/repos/OunoRentProject/OunoRentApiTests/TestAssets/testImage.png";

    var multipartFormData = CreateMultipartContent(fakeSlider, mainImagePath, mobileImagePath);
    
    // Act
    var response = await _client.PostAsync("/api/slider/", multipartFormData);
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadFromJsonAsync<SliderResponse>();

    // Assert
    Assert.NotNull(content);
    Assert.NotEqual(Guid.Empty, content.SliderId);
}

    [Fact]
    public async Task CreateSlider_ReturnsBadRequestAsync()
    {
        // Arrange
        var faker = new Faker<CreateSliderRequest>()
            .CustomInstantiator(f => new CreateSliderRequest(
                Title: f.Lorem.Sentence(),
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

        // Gerçek dosya yolları
        var mainImagePath = "/Users/ozgurgokmen/source/repos/OunoRentProject/OunoRentApiTests/TestAssets/Cat03.jpg";
        var mobileImagePath = "/Users/ozgurgokmen/source/repos/OunoRentProject/OunoRentApiTests/TestAssets/testImage.png";

        var multipartFormData = CreateMultipartContent(fakeSlider, mainImagePath, mobileImagePath);

        // Act
        var response = await _client.PostAsync("/api/slider/", multipartFormData);
        response.EnsureSuccessStatusCode();
        var sliderResponse = await response.Content.ReadFromJsonAsync<SliderResponse>();

        // Assert
        Assert.NotNull(sliderResponse);
        Assert.NotEqual(Guid.Empty, sliderResponse.SliderId);

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
        
        // Gerçek dosya yolları
        var mainImagePath = "/Users/ozgurgokmen/source/repos/OunoRentProject/OunoRentApiTests/TestAssets/Cat03.jpg";
        var mobileImagePath = "/Users/ozgurgokmen/source/repos/OunoRentProject/OunoRentApiTests/TestAssets/testImage.png";

        var multipartFormData = UpdateMultipartContent(fakeSlider, mainImagePath, mobileImagePath);
        
        var response = await _client.PutAsync($"/api/slider/{slider.SliderId}", multipartFormData);
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
        
        // Gerçek dosya yolları
        var mainImagePath = "/Users/ozgurgokmen/source/repos/OunoRentProject/OunoRentApiTests/TestAssets/Cat03.jpg";
        var mobileImagePath = "/Users/ozgurgokmen/source/repos/OunoRentProject/OunoRentApiTests/TestAssets/testImage.png";

        var multipartFormData = UpdateMultipartContent(fakeSlider, mainImagePath, mobileImagePath);
        
        var response = await _client.PutAsync($"/api/slider/{slider.SliderId}", multipartFormData);
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
    
    private MultipartFormDataContent CreateMultipartContent(CreateSliderRequest request, string mainImagePath, string mobileImagePath)
    {
        var content = new MultipartFormDataContent();

        AddStringContent(content, request.Title, "Title");
        AddStringContent(content, request.TargetUrl, "TargetUrl");
        AddStringContent(content, request.Duration.ToString(), "Duration");
        AddStringContent(content, request.OrderNumber.ToString(), "OrderNumber");
        AddStringContent(content, request.ActiveFrom.ToString("o"), "ActiveFrom");
        AddStringContent(content, request.ActiveTo.ToString("o"), "ActiveTo");
        AddStringContent(content, request.IsActive.ToString(), "IsActive");

        AddFileContent(content, mainImagePath, "MainImage");
        AddFileContent(content, mobileImagePath, "MobileImage");

        return content;
    }
    
    private MultipartFormDataContent UpdateMultipartContent(UpdateSliderRequest request, string mainImagePath, string mobileImagePath)
    {
        var content = new MultipartFormDataContent();

        AddStringContent(content, request.SliderId.ToString(), "SliderId");
        AddStringContent(content, request.Title, "Title");
        AddStringContent(content, request.TargetUrl, "TargetUrl");
        AddStringContent(content, request.Duration.ToString(), "Duration");
        AddStringContent(content, request.OrderNumber.ToString(), "OrderNumber");
        AddStringContent(content, request.ActiveFrom.ToString("o"), "ActiveFrom");
        AddStringContent(content, request.ActiveTo.ToString("o"), "ActiveTo");
        AddStringContent(content, request.IsActive.ToString(), "IsActive");

        AddFileContent(content, mainImagePath, "MainImage");
        AddFileContent(content, mobileImagePath, "MobileImage");

        return content;
    }

    private void AddStringContent(MultipartFormDataContent content, string value, string name)
    {
        if (!string.IsNullOrEmpty(value))
        {
            content.Add(new StringContent(value), name);
        }
    }

    private void AddFileContent(MultipartFormDataContent content, string filePath, string name)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            var fileBytes = File.ReadAllBytes(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, name, Path.GetFileName(filePath));
        }
    }
}