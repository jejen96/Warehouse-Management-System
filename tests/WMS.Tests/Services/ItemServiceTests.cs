using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using WMS.Application.Common;
using WMS.Application.DTOs.MasterData;
using WMS.Application.Mappings;
using WMS.Application.Services.MasterData;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Interfaces;
using Xunit;

namespace WMS.Tests.Services;

public class ItemServiceTests
{
    private readonly Mock<IRepository<Item>> _repoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly IMapper _mapper;
    private readonly ItemService _service;

    public ItemServiceTests()
    {
        _repoMock = new Mock<IRepository<Item>>();
        _uowMock = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new ItemService(_repoMock.Object, _uowMock.Object, _mapper);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsItemDto()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new Item
        {
            Id = itemId,
            ItemCode = "ITEM-001",
            ItemName = "Test Item",
            UOM = "PCS",
            MinStock = 10,
            MaxStock = 100,
            IsActive = true
        };

        _repoMock.Setup(r => r.GetByIdAsync(itemId, default))
            .ReturnsAsync(item);

        // Act
        var result = await _service.GetByIdAsync(itemId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(itemId);
        result.ItemCode.Should().Be("ITEM-001");
        result.ItemName.Should().Be("Test Item");
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(itemId, default))
            .ReturnsAsync((Item?)null);

        // Act
        var act = async () => await _service.GetByIdAsync(itemId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*{itemId}*");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateItemCode_ThrowsBusinessException()
    {
        // Arrange
        var dto = new CreateItemDto("ITEM-001", "Test Item", null, "PCS", null, 10, 100);
        var existingItem = new Item { ItemCode = "ITEM-001" };

        var items = new List<Item> { existingItem }.AsQueryable();
        _repoMock.Setup(r => r.Query()).Returns(items);

        // Act
        var act = async () => await _service.CreateAsync(dto, "testuser");

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*ITEM-001*already exists*");
    }

    [Fact]
    public async Task DeleteAsync_WhenItemExists_SoftDeletesItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new Item { Id = itemId, ItemCode = "ITEM-001", IsActive = true, IsDeleted = false };

        _repoMock.Setup(r => r.GetByIdAsync(itemId, default)).ReturnsAsync(item);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(itemId, "admin");

        // Assert
        item.IsDeleted.Should().BeTrue();
        item.IsActive.Should().BeFalse();
        item.UpdatedBy.Should().Be("admin");
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
