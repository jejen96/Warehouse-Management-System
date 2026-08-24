using FluentAssertions;
using Moq;
using WMS.Application.Common;
using WMS.Application.DTOs.Security;
using WMS.Application.Services.Security;
using WMS.Domain.Entities.Security;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;
using Xunit;

namespace WMS.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IRepository<User>> _userRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IRepository<User>>();
        _uowMock = new Mock<IUnitOfWork>();
        _tokenServiceMock = new Mock<ITokenService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _service = new AuthService(_userRepoMock.Object, _uowMock.Object,
            _tokenServiceMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@wms.local",
            PasswordHash = "hashed_password",
            Role = UserRole.Admin,
            IsActive = true
        };

        var users = new List<User> { user }.AsQueryable();
        _userRepoMock.Setup(r => r.Query()).Returns(users);
        _passwordHasherMock.Setup(p => p.Verify("Admin@123", "hashed_password")).Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateToken(user)).Returns("jwt_token_here");

        // Act
        var result = await _service.LoginAsync(new LoginDto("admin", "Admin@123"));

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("jwt_token_here");
        result.Username.Should().Be("admin");
        result.Role.Should().Be(UserRole.Admin);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsBusinessException()
    {
        // Arrange
        var user = new User
        {
            Username = "admin",
            PasswordHash = "hashed_password",
            IsActive = true
        };

        var users = new List<User> { user }.AsQueryable();
        _userRepoMock.Setup(r => r.Query()).Returns(users);
        _passwordHasherMock.Setup(p => p.Verify("wrong_password", "hashed_password")).Returns(false);

        // Act
        var act = async () => await _service.LoginAsync(new LoginDto("admin", "wrong_password"));

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*Invalid username or password*");
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ThrowsBusinessException()
    {
        // Arrange
        var users = new List<User>().AsQueryable();
        _userRepoMock.Setup(r => r.Query()).Returns(users);

        // Act
        var act = async () => await _service.LoginAsync(new LoginDto("nonexistent", "password"));

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*Invalid username or password*");
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateUsername_ThrowsBusinessException()
    {
        // Arrange
        var existingUser = new User { Username = "admin" };
        var users = new List<User> { existingUser }.AsQueryable();
        _userRepoMock.Setup(r => r.Query()).Returns(users);

        var dto = new CreateUserDto("admin", "new@email.com", "Password@1", UserRole.Operator, null);

        // Act
        var act = async () => await _service.RegisterAsync(dto, "system");

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*admin*already exists*");
    }
}
