using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Baqei.Domain.Interfaces;
using Baqei.Domain.Entities;
using System.Linq;
using Baqei.Application.Services;

namespace Baqei.Tests.Unit.Services;

public class PlotServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        var mockRepo = new Mock<IPlotRepository>();
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Plot>
        {
            new Plot { Id = 1, Title = "A", Size = 100, Coordinates = "0,0" },
            new Plot { Id = 2, Title = "B", Size = 200, Coordinates = "1,1" }
        });

        var service = new PlotService(mockRepo.Object);
        var result = (await service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0].Title);
    }
}
