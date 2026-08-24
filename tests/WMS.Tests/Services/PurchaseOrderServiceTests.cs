using AutoMapper;
using FluentAssertions;
using Moq;
using WMS.Application.Common;
using WMS.Application.DTOs.Inbound;
using WMS.Application.Mappings;
using WMS.Application.Services.Inbound;
using WMS.Domain.Entities.Inbound;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;
using Xunit;

namespace WMS.Tests.Services;

public class PurchaseOrderServiceTests
{
    private readonly Mock<IRepository<PurchaseOrder>> _repoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly IMapper _mapper;
    private readonly PurchaseOrderService _service;

    public PurchaseOrderServiceTests()
    {
        _repoMock = new Mock<IRepository<PurchaseOrder>>();
        _uowMock = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new PurchaseOrderService(_repoMock.Object, _uowMock.Object, _mapper);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPONotFound_ThrowsNotFoundException()
    {
        // Arrange
        var poId = Guid.NewGuid();
        var emptyList = new List<PurchaseOrder>().AsQueryable();
        _repoMock.Setup(r => r.Query()).Returns(emptyList);

        // Act
        var act = async () => await _service.GetByIdAsync(poId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenPOIsNotDraft_ThrowsBusinessException()
    {
        // Arrange
        var poId = Guid.NewGuid();
        var po = new PurchaseOrder { Id = poId, Status = POStatus.Confirmed };

        _repoMock.Setup(r => r.GetByIdAsync(poId, default)).ReturnsAsync(po);

        // Act
        var act = async () => await _service.DeleteAsync(poId, "admin");

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*Draft*");
    }

    [Fact]
    public async Task DeleteAsync_WhenPOIsDraft_SoftDeletesPO()
    {
        // Arrange
        var poId = Guid.NewGuid();
        var po = new PurchaseOrder { Id = poId, Status = POStatus.Draft, IsDeleted = false };

        _repoMock.Setup(r => r.GetByIdAsync(poId, default)).ReturnsAsync(po);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(poId, "admin");

        // Assert
        po.IsDeleted.Should().BeTrue();
        po.UpdatedBy.Should().Be("admin");
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
