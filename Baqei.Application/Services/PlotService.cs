using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baqei.Application.DTOs;
using Baqei.Domain.Entities;
using Baqei.Domain.Interfaces;

namespace Baqei.Application.Services;

public class PlotService
{
    private readonly IPlotRepository _repository;

    public PlotService(IPlotRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PlotDto>> GetAllAsync()
    {
        var plots = await _repository.GetAllAsync();
        var dtos = plots.Select(p => new PlotDto(p.Id, p.Title, p.Size, p.Coordinates));
        return dtos;
    }

    public async Task<PlotDto?> GetByIdAsync(int id)
    {
        var plot = await _repository.GetByIdAsync(id);
        if (plot == null) return null;
        return new PlotDto(plot.Id, plot.Title, plot.Size, plot.Coordinates);
    }

    public async Task<PlotDto> CreateAsync(PlotDto dto)
    {
        var plot = new Plot
        {
            Title = dto.Title,
            Size = dto.Size,
            Coordinates = dto.Coordinates
        };

        var created = await _repository.CreateAsync(plot);
        return new PlotDto(created.Id, created.Title, created.Size, created.Coordinates);
    }
}
