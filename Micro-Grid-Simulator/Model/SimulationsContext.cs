using Microsoft.EntityFrameworkCore;

namespace Micro_Grid_Simulator.Model;

public class SimulationsContext : DbContext
{
    
    public SimulationsContext(DbContextOptions<SimulationsContext> options)
        : base(options)
    {
    }
}