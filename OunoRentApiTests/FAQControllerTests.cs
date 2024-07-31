using System.Net;
using System.Net.Http.Json;
using Bogus;
using BusinessLayer.Middlewares;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared.DTO.FAQ.Request;
using Shared.DTO.FAQ.Response;

namespace OunoRentApiTests;

public class FAQControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public FAQControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region CreateFAQTests

    [Fact]
    public async Task CreateFAQ_ReturnsSuccess()
    {
        //Arrange
        var faker = new Faker<CreateFAQRequest>()
            .CustomInstantiator(f => new CreateFAQRequest(
                Label: f.Lorem.Sentence(),
                Text: f.Lorem.Text(),
                OrderNumber: f.Random.Int(1, 1000),
                IsActive: f.Random.Bool()
            ));

        var faq = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("api/faq", faq);
        var content = await response.Content.ReadFromJsonAsync<FAQResponse>();

        //Assert
        response.EnsureSuccessStatusCode();
        Assert.NotEqual(Guid.Empty, content.FAQId);
    }

    [Fact]
    public async Task CreateFAQ_ReturnsOrderNumberExist()
    {
        //Arrange
        var existingFaq = await GetExistFAQ();
        var faker = new Faker<CreateFAQRequest>()
            .CustomInstantiator(f => new CreateFAQRequest(
                Label: f.Lorem.Sentence(),
                Text: f.Lorem.Text(),
                OrderNumber: existingFaq.OrderNumber,
                IsActive: f.Random.Bool()
            ));

        var faq = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("api/faq", faq);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Contains(FAQExceptionMessages.OrderNumberConflict, content);
    }

    [Fact]
    public async Task CreateFAQ_ReturnsLabelExist()
    {
        //Arrange
        var existingFaq = await GetExistFAQ();
        var faker = new Faker<CreateFAQRequest>()
            .CustomInstantiator(f => new CreateFAQRequest(
                Label: existingFaq.Label,
                Text: f.Lorem.Text(),
                OrderNumber: f.Random.Int(1, 1000),
                IsActive: f.Random.Bool()
            ));

        var faq = faker.Generate();

        //Act
        var response = await _client.PostAsJsonAsync("api/faq", faq);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Contains(FAQExceptionMessages.LabelConflict, content);
    }

    #endregion

    #region GetFAQTests

    [Fact]
    public async Task GetFAQ_ReturnsSuccess()
    {
        //Arrange
        var faqList = await GetFAQList();

        //Act
        var response = await _client.GetAsync($"api/faq/{faqList.FirstOrDefault().FAQId}");
        var content = await response.Content.ReadFromJsonAsync<GetFAQResponse>();

        //Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(faqList.FirstOrDefault().FAQId, content.FAQId);
        Assert.Equal(faqList.FirstOrDefault().OrderNumber, content.OrderNumber);
        Assert.Equal(faqList.FirstOrDefault().IsActive, content.IsActive);
        Assert.Equal(faqList.FirstOrDefault().Label, content.Label);
        Assert.NotEmpty(content.Text);
    }

    [Fact]
    public async Task GetFAQ_ReturnsNotFound()
    {
        //Arrange

        //Act
        var response = await _client.GetAsync($"api/faq/{Guid.Empty}");
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains(FAQExceptionMessages.NotFound, content);
    }

    #endregion

    #region GetFAQsTests

    [Fact]
    public async Task GetFAQs_ReturnsSuccess()
    {
        //Arrange

        //Act
        var response = await _client.GetAsync("api/faq");
        var content = await response.Content.ReadFromJsonAsync<List<GetFAQsResponse>>();

        //Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(content);
    }

    [Fact]
    public async Task GetActiveFAQs_ReturnsSuccess()
    {
        //Arrange

        //Act
        var response = await _client.GetAsync("api/faq/GetActive");
        var content = await response.Content.ReadFromJsonAsync<List<GetFAQsResponse>>();

        //Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(content);
        foreach (var item in content)
        {
            Assert.True(item.IsActive);
        }
    }

    #endregion

    #region UpdateFAQTests

    [Fact]
    public async Task UpdateFAQ_ReturnsSuccess()
    {
        //Arrange
        var existingFaq = await GetExistFAQ();
        var faker = new Faker<UpdateFAQRequest>()
            .CustomInstantiator(f => new UpdateFAQRequest(
                FAQId: existingFaq.FAQId,
                Label: f.Lorem.Sentence(),
                Text: f.Lorem.Text(),
                OrderNumber: f.Random.Int(1, 1000),
                IsActive: f.Random.Bool()
            ));

        var faq = faker.Generate();

        //Act
        var response = await _client.PutAsJsonAsync($"api/faq/{faq.FAQId}", faq);
        var content = await response.Content.ReadFromJsonAsync<FAQResponse>();

        //Assert
        response.EnsureSuccessStatusCode();
        Assert.NotEqual(Guid.Empty, content.FAQId);
    }


    [Fact]
    public async Task UpdateFAQ_ReturnsOrderNumberExist()
    {
        //Arrange
        var existFaq = await GetExistFAQ();
        var faqList = await GetFAQList();
        var existOrderNumber = faqList.FirstOrDefault(f => f.FAQId != existFaq.FAQId).OrderNumber;

        var faker = new Faker<UpdateFAQRequest>()
            .CustomInstantiator(f => new UpdateFAQRequest(
                FAQId: existFaq.FAQId,
                Label: f.Lorem.Sentence(),
                Text: f.Lorem.Text(),
                OrderNumber: existOrderNumber,
                IsActive: f.Random.Bool()
            ));

        var faq = faker.Generate();

        //Act
        var response = await _client.PutAsJsonAsync($"api/faq/{faq.FAQId}", faq);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Contains(FAQExceptionMessages.OrderNumberConflict, content);
    }
    
    [Fact]
    public async Task UpdateFAQ_ReturnsLabelExist()
    {
        //Arrange
        var existFaq = await GetExistFAQ();
        var faqList = await GetFAQList();
        var existLabel = faqList.FirstOrDefault(f => f.FAQId != existFaq.FAQId).Label;

        var faker = new Faker<UpdateFAQRequest>()
            .CustomInstantiator(f => new UpdateFAQRequest(
                FAQId: existFaq.FAQId,
                Label: existLabel,
                Text: f.Lorem.Text(),
                OrderNumber: f.Random.Int(1,1000),
                IsActive: f.Random.Bool()
            ));

        var faq = faker.Generate();

        //Act
        var response = await _client.PutAsJsonAsync($"api/faq/{faq.FAQId}", faq);
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Contains(FAQExceptionMessages.LabelConflict, content);
    }

    #endregion

    #region DeleteFAQTests

    [Fact]
    public async Task DeleteFAQ_ReturnsSuccess()
    {
        //Arrange
        var existFaq = await GetExistFAQ();
        
        //Act
        var response = await _client.DeleteAsync($"api/FAQ/{existFaq.FAQId}");
        var content = await response.Content.ReadFromJsonAsync<FAQResponse>();
        
        //Assert
        response.EnsureSuccessStatusCode();
        Assert.NotEqual(Guid.Empty, content.FAQId);
    }
    
    [Fact]
    public async Task DeleteFAQ_ReturnsNotFound()
    {
        //Arrange
        
        //Act
        var response = await _client.DeleteAsync($"api/FAQ/{Guid.Empty}");
        var content = await response.Content.ReadAsStringAsync();
        
        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains(FAQExceptionMessages.NotFound, content);
    }

    #endregion

    #region Utils
    
    private async Task<GetFAQResponse> GetExistFAQ()
    {
        var faqList = await GetFAQList();

        var response = await _client.GetAsync($"api/faq/{faqList.FirstOrDefault().FAQId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<GetFAQResponse>();

        return content;
    }

    private async Task<List<GetFAQsResponse>> GetFAQList()
    {
        var response = await _client.GetAsync("api/faq");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<List<GetFAQsResponse>>();

        return content;
    }
    
    #endregion
}
