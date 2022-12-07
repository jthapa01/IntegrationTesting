using System.Net;
using System.Net.Http.Json;
using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Xunit;

namespace Customers.Api.Tests.Integration.CustomerController;

public class GetAllCustomerControllerTests1 : IClassFixture<CustomerApiFactory>
{
    private readonly HttpClient _client;

    private readonly Faker<CustomerRequest> _customer = new Faker<CustomerRequest>()
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.FullName, faker => faker.Person.FullName)
        .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGithubUser)
        .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

    public GetAllCustomerControllerTests1(CustomerApiFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsAllCustomers_WhenCustomersExist()
    {
        // Arrange
        var customer = _customer.Generate();
        var customerResponse = await _client.PostAsJsonAsync("customers", customer);
        var createdCustomer =  await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        // Act 
        var response = await _client.GetAsync("customers");
        
        // Assert
        var retrievedCustomer = await response.Content.ReadFromJsonAsync<GetAllCustomersResponse>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        retrievedCustomer!.Customers.Single().Should().BeEquivalentTo(createdCustomer);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyResults_WhenNoCustomersExist()
    {
        // Act
        var response = await _client.GetAsync("customers");
        
        // Assert
        var retrievedCustomer = await response.Content.ReadFromJsonAsync<GetAllCustomersResponse>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        retrievedCustomer!.Customers.Should().BeEmpty();
    }
}