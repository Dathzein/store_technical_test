using FluentAssertions;
using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Tests.Unit.Entities;

public class ProductTests
{
    [Fact]
    public void Product_Creation_ShouldSetProperties()
    {
        // Arrange & Act
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 100.50m,
            Stock = 10,
            CategoryId = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        product.Id.Should().Be(1);
        product.Name.Should().Be("Test Product");
        product.Description.Should().Be("Test Description");
        product.Price.Should().Be(100.50m);
        product.Stock.Should().Be(10);
        product.CategoryId.Should().Be(1);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Product_WithCategory_ShouldHaveRelationship()
    {
        // Arrange & Act
        var category = new Category { Id = 1, Name = "Test Category" };
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            CategoryId = 1,
            Category = category
        };

        // Assert
        product.Category.Should().NotBeNull();
        product.Category!.Id.Should().Be(1);
        product.Category.Name.Should().Be("Test Category");
    }
}

public class CategoryTests
{
    [Fact]
    public void Category_Creation_ShouldSetProperties()
    {
        // Arrange & Act
        var category = new Category
        {
            Id = 1,
            Name = "Electronics",
            Description = "Electronic devices",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        category.Id.Should().Be(1);
        category.Name.Should().Be("Electronics");
        category.Description.Should().Be("Electronic devices");
        category.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}

public class UserTests
{
    [Fact]
    public void User_Creation_ShouldSetProperties()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            RoleId = 1
        };

        // Assert
        user.Id.Should().Be(1);
        user.Username.Should().Be("testuser");
        user.Email.Should().Be("test@example.com");
        user.PasswordHash.Should().Be("hashedpassword");
        user.RoleId.Should().Be(1);
    }

    [Fact]
    public void User_WithRole_ShouldHaveRelationship()
    {
        // Arrange & Act
        var role = new Role { Id = 1, Name = "Admin" };
        var user = new User
        {
            Id = 1,
            Username = "admin",
            RoleId = 1,
            Role = role
        };

        // Assert
        user.Role.Should().NotBeNull();
        user.Role!.Name.Should().Be("Admin");
    }
}

