using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Baqei.Domain.Entities;
using Baqei.Domain.Interfaces;
using Baqei.Infrastructure.Data;

namespace Baqei.Infrastructure.Repositories;

public class PlotRepository : IPlotRepository
{
    private readonly AppDbContext _context;

    public PlotRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Plot>> GetAllAsync()
    {
        var plots = await _context.Plots.ToListAsync();
        return plots;
    }

    public async Task<Plot?> GetByIdAsync(int id)
    {
        var plot = await _context.Plots.FindAsync(id);
        return plot;
    }

    public async Task<Plot> CreateAsync(Plot plot)
    {
        _context.Plots.Add(plot);
        await _context.SaveChangesAsync();
        return plot;
    }
}
