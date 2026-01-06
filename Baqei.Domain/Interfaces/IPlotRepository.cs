using Baqei.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqei.Domain.Interfaces
{
    public interface IPlotRepository
    {
        Task<IEnumerable<Plot>> GetAllAsync();
        Task<Plot?> GetByIdAsync(int id);
        Task<Plot> CreateAsync(Plot plot);
    }
}
