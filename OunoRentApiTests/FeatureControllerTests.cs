using System.Net;
using System.Net.Http.Json;
using Bogus;
using BusinessLayer.Middlewares;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared.DTO.Category.Response;
using Shared.DTO.Feature.Request;
using Shared.DTO.Feature.Response;
using Shared.DTO.SubCategory.Response;

namespace OunoRentApiTests;

public class FeatureControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public FeatureControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region CreateFeaturesTests

    [Fact]
    public async Task CreateFeatures_ReturnsSuccess()
    {
        //Arrange
        var category = await GetCategory();
        var subCategory = await GetSubcategory(category.CategoryId);

        var faker = new Faker<CreateFeatureRequest>()
            .CustomInstantiator(f => new CreateFeatureRequest(
                FeatureName: f.Name.JobTitle(),
                FeatureType: f.Name.JobType(),
                CategoryId: category.CategoryId,
                SubCategoryId: subCategory.SubCategoryId,
                IsActive: f.Random.Bool()
            ));

        var feature = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("/api/Feature", feature);
        var content = await response.Content.ReadFromJsonAsync<FeatureResponse>();

        //Assert
        response.EnsureSuccessStatusCode();
        Assert.NotEqual(Guid.Empty, content.FeatureId);
    }
    
    [Fact]
    public async Task CreateFeatures_ReturnsCategoryNotFound()
    {
        //Arrange
        var category = await GetCategory();
        var subCategory = await GetSubcategory(category.CategoryId);

        var faker = new Faker<CreateFeatureRequest>()
            .CustomInstantiator(f => new CreateFeatureRequest(
                FeatureName: f.Name.JobTitle(),
                FeatureType: f.Name.JobType(),
                CategoryId: Guid.Empty,
                SubCategoryId: subCategory.SubCategoryId,
                IsActive: f.Random.Bool()
            ));

        var feature = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("/api/Feature", feature);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.Contains(CategoryExceptionMessages.NotFound, content);
    }
    
    [Fact]
    public async Task CreateFeatures_ReturnsSubCategoryNotFound()
    {
        //Arrange
        var category = await GetCategory();

        var faker = new Faker<CreateFeatureRequest>()
            .CustomInstantiator(f => new CreateFeatureRequest(
                FeatureName: f.Name.JobTitle(),
                FeatureType: f.Name.JobType(),
                CategoryId: category.CategoryId,
                SubCategoryId: Guid.Empty,
                IsActive: f.Random.Bool()
            ));

        var feature = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("/api/Feature", feature);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.Contains(CategoryExceptionMessages.NotFound, content);
    }

    #endregion

    #region UpdateFeaturesTests

    [Fact]
    public async Task UpdateFeature_ReturnsSuccess()
    {
        //Arrange
        var existFeature = await GetFeature();
        
        var faker = new Faker<UpdateFeatureRequest>()
            .CustomInstantiator(f => new UpdateFeatureRequest(
                FeatureId: existFeature.FeatureId,
                FeatureName: f.Name.JobTitle(),
                FeatureType: f.Name.JobType(),
                CategoryId: existFeature.Category.CategoryId,
                SubCategoryId: existFeature.SubCategory.SubCategoryId,
                IsActive: f.Random.Bool()
            ));

        var feature = faker.Generate();
        
        //Act
        var response = await _client.PutAsJsonAsync($"/api/feature/{feature.FeatureId}", feature);
        var content = await response.Content.ReadFromJsonAsync<FeatureResponse>();
        
        //Assert
        response.EnsureSuccessStatusCode();
        Assert.NotEqual(Guid.Empty, content.FeatureId);
    }

    [Fact]
    public async Task UpdateFeature_ReturnsNotFound()
    {
        //Arrange
        var existFeature = await GetFeature();
        
        var faker = new Faker<UpdateFeatureRequest>()
            .CustomInstantiator(f => new UpdateFeatureRequest(
                FeatureId: Guid.Empty,
                FeatureName: f.Name.JobTitle(),
                FeatureType: f.Name.JobType(),
                CategoryId: existFeature.Category.CategoryId,
                SubCategoryId: existFeature.SubCategory.SubCategoryId,
                IsActive: f.Random.Bool()
            ));

        var feature = faker.Generate();
        
        //Act
        var response = await _client.PutAsJsonAsync($"/api/feature/{feature.FeatureId}", feature);
        var content = await response.Content.ReadAsStringAsync();
        
        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains(FeatureExceptionMessages.NotFound, content);
    }
    
    [Fact]
    public async Task UpdateFeature_ReturnsCategoryNotFound()
    {
        //Arrange
        var existFeature = await GetFeature();
        
        var faker = new Faker<UpdateFeatureRequest>()
            .CustomInstantiator(f => new UpdateFeatureRequest(
                FeatureId: existFeature.FeatureId,
                FeatureName: f.Name.JobTitle(),
                FeatureType: f.Name.JobType(),
                CategoryId: Guid.Empty, 
                SubCategoryId: existFeature.SubCategory.SubCategoryId,
                IsActive: f.Random.Bool()
            ));

        var feature = faker.Generate();
        
        //Act
        var response = await _client.PutAsJsonAsync($"/api/feature/{feature.FeatureId}", feature);
        var content = await response.Content.ReadAsStringAsync();
        
        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains(CategoryExceptionMessages.NotFound, content);
    }
    
    [Fact]
    public async Task UpdateFeature_ReturnsSubCategoryNotFound()
    {
        //Arrange
        var existFeature = await GetFeature();
        
        var faker = new Faker<UpdateFeatureRequest>()
            .CustomInstantiator(f => new UpdateFeatureRequest(
                FeatureId: existFeature.FeatureId,
                FeatureName: f.Name.JobTitle(),
                FeatureType: f.Name.JobType(),
                CategoryId: existFeature.Category.CategoryId, 
                SubCategoryId: Guid.Empty,
                IsActive: f.Random.Bool()
            ));

        var feature = faker.Generate();
        
        //Act
        var response = await _client.PutAsJsonAsync($"/api/feature/{feature.FeatureId}", feature);
        var content = await response.Content.ReadAsStringAsync();
        
        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains(CategoryExceptionMessages.NotFound, content);
    }
    
    #endregion

    #region DeleteFeaturesTests

    [Fact]
    public async Task DeleteFeature_ReturnsSuccess()
    {
        //Arrange
        var feature = await GetFeature();
        
        //Act
        var response = await _client.DeleteAsync($"api/Feature/{feature.FeatureId}");
        var content = await response.Content.ReadFromJsonAsync<FeatureResponse>();
        
        //Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(feature.FeatureId, content.FeatureId);
    }
    
    [Fact]
    public async Task DeleteFeature_ReturnsNotFound()
    {
        //Arrange
        
        //Act
        var response = await _client.DeleteAsync($"api/Feature/{Guid.Empty}");
        var content = await response.Content.ReadAsStringAsync();
        
        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains(FeatureExceptionMessages.NotFound, content);
    }

    #endregion

    #region GetAllFeaturesTests

    [Fact]
    public async Task GetAllFeatures_ReturnsSuccess()
    {
        //Arrange
        
        //Act
        var response = await _client.GetAsync("api/Feature");
        var content = await response.Content.ReadFromJsonAsync<List<GetFeaturesResponse>>();
        
        //Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(content);
    }
    
    [Fact]
    public async Task GetActiveAllFeatures_ReturnsSuccess()
    {
        //Arrange
        
        //Act
        var response = await _client.GetAsync("api/Feature/GetActive");
        var content = await response.Content.ReadFromJsonAsync<List<GetFeaturesResponse>>();
        
        //Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(content);
    }
    #endregion

    #region GetFeatureTests
    
    [Fact]
    public async Task GetFeature_ReturnsSuccess()
    {
        //Arrange
        var features = await GetFeatures();
        
        //Act
        var response = await _client.GetAsync($"api/Feature/{features.FirstOrDefault().FeatureId}");
        var content = await response.Content.ReadFromJsonAsync<GetFeatureResponse>();
        
        //Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.FeatureId);
        Assert.NotNull(content.Category);
        Assert.NotNull(content.SubCategory);
    }

    
    [Fact]
    public async Task GetFeature_ReturnsNotFound()
    {
        //Arrange
        
        //Act
        var response = await _client.GetAsync($"api/Feature/{Guid.Empty}");
        var content = await response.Content.ReadAsStringAsync();
        
        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.Contains(FeatureExceptionMessages.NotFound, content);
    }

    #endregion
    private async Task<List<GetFeaturesResponse>> GetFeatures()
    {
        var response = await _client.GetAsync("api/Feature");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<List<GetFeaturesResponse>>();
        
        return content;
    }

    private async Task<GetFeatureResponse> GetFeature()
    {
        var featureId = (await GetFeatures()).FirstOrDefault().FeatureId;

        var response = await _client.GetAsync($"api/Feature/{featureId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<GetFeatureResponse>();

        return content;
    }
    
    private async Task<List<GetCategoriesResponse>> GetCategories()
    {
        var response = await _client.GetAsync("api/Category");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<List<GetCategoriesResponse>>();

        return content;
    }

    private async Task<GetCategoryResponse> GetCategory()
    {
        var categoryId = (await GetCategories()).FirstOrDefault(f => f.SubCategories.Count > 0).CategoryId;

        var response = await _client.GetAsync($"api/Category/{categoryId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<GetCategoryResponse>();
        return content;
    }

    private async Task<List<GetSubCategoriesResponse>> GetSubcategories(Guid categoryId)
    {
        var response = await _client.GetAsync($"api/Category/{categoryId}/subcategory");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<List<GetSubCategoriesResponse>>();

        return content;
    }

    private async Task<GetSubCategoryResponse> GetSubcategory(Guid categoryId)
    {
        var subCategories = await GetSubcategories(categoryId);
        var response =
            await _client.GetAsync(
                $"api/Category/{categoryId}/subcategory/{subCategories.FirstOrDefault().SubCategoryId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<GetSubCategoryResponse>();

        return content;
    }
}