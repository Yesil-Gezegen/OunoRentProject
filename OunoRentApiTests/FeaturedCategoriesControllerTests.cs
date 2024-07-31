using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using Bogus;
using BusinessLayer.Middlewares;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared.DTO.Category.Response;
using Shared.DTO.FeaturedCategories.Request;
using Shared.DTO.FeaturedCategories.Response;

namespace OunoRentApiTests;

public class FeaturedCategoriesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public FeaturedCategoriesControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region CreateFeaturedCategoryTests

    [Fact]
    public async Task CreatFeaturedCategory_ReturnsSuccess()
    {
        //Arrange
        var category = await ExplictCategory();

        var faker = new Faker<CreateFeaturedCategoryRequest>()
            .CustomInstantiator(f => new CreateFeaturedCategoryRequest(
                CategoryId: category.CategoryId,
                OrderNumber: f.Random.Int(1, 1000),
                IsActive: f.Random.Bool()
            ));

        var featuredCategory = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("/api/featuredcategory/", featuredCategory);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<FeaturedCategoryResponse>();

        //Assert
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.FeaturedCategoryId);
    }

    [Fact]
    public async Task CreateFeaturedCategory_ReturnsOrderNumberExist()
    {
        //Arrange
        var category = await ExplictCategory();
        var existingFeaturedCategory = await GetFeaturedCategory();

        var faker = new Faker<CreateFeaturedCategoryRequest>()
            .CustomInstantiator(f => new CreateFeaturedCategoryRequest(
                OrderNumber: existingFeaturedCategory.OrderNumber,
                IsActive: f.Random.Bool(),
                CategoryId: category.CategoryId
            ));

        var featuredCategory = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("/api/featuredcategory/", featuredCategory);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(content);
        Assert.Contains(
            FeaturedCategoryExceptionMessages.OrderNumberConflict,
            content);
    }

    [Fact]
    public async Task CreateFeaturedCategory_ReturnsSameCategoryExist()
    {
        //Arrange
        var existingFeaturedCategory = await GetFeaturedCategory();

        var faker = new Faker<CreateFeaturedCategoryRequest>()
            .CustomInstantiator(f => new CreateFeaturedCategoryRequest(
                OrderNumber: f.Random.Int(1, 1000),
                IsActive: f.Random.Bool(),
                CategoryId: existingFeaturedCategory.GetCategoryResponse.CategoryId
            ));

        var featuredCategory = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("/api/featuredcategory/", featuredCategory);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(content);
        Assert.Contains(
            FeaturedCategoryExceptionMessages.CategoryConflict,
            content);
    }

    [Fact]
    public async Task CreateFeaturedCategory_ReturnsCategoryNotFound()
    {
        //Arrange
        var faker = new Faker<CreateFeaturedCategoryRequest>()
            .CustomInstantiator(f => new CreateFeaturedCategoryRequest(
                OrderNumber: f.Random.Int(1, 1000),
                IsActive: f.Random.Bool(),
                CategoryId: Guid.Empty
            ));

        var featuredCategory = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("/api/featuredcategory/", featuredCategory);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.Contains(
            CategoryExceptionMessages.NotFound,
            content);
    }

    #endregion

    #region UpdateFeaturedCategoryTests

    [Fact]
    public async Task UpdateFeatureCategory_ReturnsSuccess()
    {
        //Arrange
        var entity = await GetFeaturedCategory();

        var faker = new Faker<UpdateFeaturedCategoryRequest>()
            .CustomInstantiator(f => new UpdateFeaturedCategoryRequest(
                FeaturedCategoryId: entity.FeaturedCategoryId,
                CategoryId: entity.GetCategoryResponse.CategoryId,
                OrderNumber: f.Random.Int(1, 1000),
                IsActive: f.Random.Bool()
            ));

        var featuredCategory = faker.Generate();

        //Act
        var response = await _client.PutAsJsonAsync($"/api/featuredcategory/{featuredCategory.FeaturedCategoryId}",
            featuredCategory);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<FeaturedCategoryResponse>();

        //Assert
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.FeaturedCategoryId);
    }

    [Fact]
    public async Task UpdateFeaturedCategory_ReturnsCategoryNotFound()
    {
        //Arrange
        var entity = await GetFeaturedCategory();

        var faker = new Faker<UpdateFeaturedCategoryRequest>()
            .CustomInstantiator(f => new UpdateFeaturedCategoryRequest(
                FeaturedCategoryId: entity.FeaturedCategoryId,
                CategoryId: Guid.Empty,
                OrderNumber: f.Random.Int(1, 1000),
                IsActive: f.Random.Bool()
            ));

        var featuredCategory = faker.Generate();

        //Act
        var response = await _client.PutAsJsonAsync($"/api/featuredcategory/{featuredCategory.FeaturedCategoryId}",
            featuredCategory);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.Contains(CategoryExceptionMessages.NotFound, content);
    }

    [Fact]
    public async Task UpdateFeatureCategory_ReturnsSameCategoryExist()
    {
        //Arrange
        var entity = await GetFeaturedCategory(fc => fc.IsActive);

        var existingFeaturdCategory = await GetFeaturedCategory(fc =>
            fc.FeaturedCategoryId != entity.FeaturedCategoryId);

        var existingCategoryId = existingFeaturdCategory.GetCategoryResponse.CategoryId;

        var faker = new Faker<UpdateFeaturedCategoryRequest>()
            .CustomInstantiator(f => new UpdateFeaturedCategoryRequest(
                FeaturedCategoryId: entity.FeaturedCategoryId,
                CategoryId: existingCategoryId,
                OrderNumber: f.Random.Int(1, 1000),
                IsActive: f.Random.Bool()
            ));

        var featuredCategory = faker.Generate();

        //Act
        var response = await _client.PutAsJsonAsync($"/api/featuredcategory/{featuredCategory.FeaturedCategoryId}",
            featuredCategory);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(content);
        Assert.Contains(FeaturedCategoryExceptionMessages.CategoryConflict, content);
    }

    [Fact]
    public async Task UpdateFeatureCategory_ReturnsSameOrderNumberExist()
    {
        //Arrange
        var entity = await GetFeaturedCategory();

        var existingOrderNumber = (await GetFeaturedCategory(fc =>
            fc.FeaturedCategoryId != entity.FeaturedCategoryId)).OrderNumber;

        var faker = new Faker<UpdateFeaturedCategoryRequest>()
            .CustomInstantiator(f => new UpdateFeaturedCategoryRequest(
                FeaturedCategoryId: entity.FeaturedCategoryId,
                CategoryId: entity.GetCategoryResponse.CategoryId,
                OrderNumber: existingOrderNumber,
                IsActive: f.Random.Bool()
            ));

        var featuredCategory = faker.Generate();

        //Act
        var response = await _client.PutAsJsonAsync($"/api/featuredcategory/{featuredCategory.FeaturedCategoryId}",
            featuredCategory);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(content);
        Assert.Contains(FeaturedCategoryExceptionMessages.OrderNumberConflict, content);
    }

    #endregion

    #region DeleteFeaturedCategoryTests

    [Fact]
    public async Task DeleteFeaturedCategory_ReturnsSuccess()
    {
        //Arrange
        var entity = await GetFeaturedCategory();

        //Act
        var response = await _client.DeleteAsync($"api/FeaturedCategory/{entity.FeaturedCategoryId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<Guid>();

        //Assert
        Assert.NotNull(content);
        Assert.Equal(entity.FeaturedCategoryId, content);
    }

    [Fact]
    public async Task DeleteFeaturedCategory_ReturnsNotFound()
    {
        //Arrange

        //Act
        var response = await _client.DeleteAsync($"api/FeaturedCategory/{Guid.Empty}");
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.Contains(CategoryExceptionMessages.NotFound, content);
    }

    #endregion

    #region GetFeaturedCategoriesTests

    [Fact]
    public async Task GetFeaturedCategories_ReturnsSuccess()
    {
        //Arrange

        //Act
        var response = await _client.GetAsync("api/FeaturedCategory");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<List<GetFeaturedCategoriesResponse>>();

        //Assert
        Assert.NotNull(content);
    }

    [Fact]
    public async Task GetActiveFeaturedCategories_ReturnsSuccess()
    {
        //Act
        var response = await _client.GetAsync("/api/MenuItem/GetActive");
        var content = await response.Content.ReadFromJsonAsync<List<GetFeaturedCategoriesResponse>>();
        
        //Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(content);
        foreach (var item in content)
        {
            Assert.True(item.IsActive);
        }
    }
    
    #endregion

    #region GetFeaturedCategoryTests

    [Fact]
    public async Task GetFeaturedCategory_ReturnsSuccess()
    {
        //Arrange
        var entity = await GetFeaturedCategory();

        //Act
        var response = await _client.GetAsync($"api/FeaturedCategory/{entity.FeaturedCategoryId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<GetFeaturedCategoryResponse>();

        //Assert
        Assert.NotNull(content);
        Assert.Equal(entity.FeaturedCategoryId, content.FeaturedCategoryId);
    }

    [Fact]
    public async Task GetFeaturedCategory_ReturnsNotFound()
    {
        //Arrange

        //Act
        var response = await _client.GetAsync($"api/FeaturedCategory/{Guid.Empty}");
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.Contains(CategoryExceptionMessages.NotFound, content);
    }

    #endregion

    #region Utils

    private async Task<GetCategoriesResponse> ExplictCategory()
    {
        var categoriesResponse = await _client.GetAsync("/api/category/");
        categoriesResponse.EnsureSuccessStatusCode();
        var categoriesContent = await categoriesResponse.Content.ReadFromJsonAsync<List<GetCategoriesResponse>>();

        var featuredCategoriesResponse = await _client.GetAsync("/api/featuredcategory");
        featuredCategoriesResponse.EnsureSuccessStatusCode();
        var featuredCategoriesContent =
            await featuredCategoriesResponse.Content.ReadFromJsonAsync<List<GetFeaturedCategoriesResponse>>();

        GetCategoriesResponse category = null;

        foreach (var item in categoriesContent)
        {
            if (featuredCategoriesContent.Any(c => 
                    c.GetCategoryResponse.CategoryId == item.CategoryId))
                continue;

            category = item;
            break;
        }

        if (category == null)
            throw new NotFoundException(
                "No categories have not been added to the featured products. Add new category and run the test again");

        return category;
    }

    private async Task<GetFeaturedCategoriesResponse?> GetFeaturedCategory(
        Func<GetFeaturedCategoriesResponse, bool>? predicate = null)
    {
        var featuredCategoriesResponse = await _client.GetAsync("api/FeaturedCategory");
        featuredCategoriesResponse.EnsureSuccessStatusCode();

        var featuredCategoriesContent =
            await featuredCategoriesResponse.Content.ReadFromJsonAsync<List<GetFeaturedCategoriesResponse>>();

        return predicate == null
            ? featuredCategoriesContent.FirstOrDefault()
            : featuredCategoriesContent.FirstOrDefault(predicate);
    }

    #endregion
}