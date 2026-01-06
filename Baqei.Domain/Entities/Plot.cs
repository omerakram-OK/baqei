using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqei.Domain.Entities
{
    public class Plot
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
 
        public double Size { get; set; }

        public string Coordinates { get; set; } = string.Empty;
    }
}
