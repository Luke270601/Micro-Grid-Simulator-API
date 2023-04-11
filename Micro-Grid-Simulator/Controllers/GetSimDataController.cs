using System.Text.Json;
using Micro_Grid_Management.Micro_Grid;
using Micro_Grid_Simulator.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Micro_Grid_Simulator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GetSimData : ControllerBase
{
    private readonly SimulationsContext _simulationsContext;

    public GetSimData(SimulationsContext simulationsContext)
    {
        _simulationsContext = simulationsContext;
    }


    [HttpGet("GetNames")]
    public async Task<ActionResult<IEnumerable<SimulationsModel>>> GetAllProducts()
    {
        var simulations = await _simulationsContext.Simulations.Select(s =>
            new { s.SimId, s.Date, s.TurbineCount, s.PanelCount, s.HouseCount, s.Duration }).ToListAsync();
        return Ok(simulations);
    }

    [HttpGet("GetData/{simId}")]
    public JsonResult GetSimInfo(int simId)
    {
        var simulations = _simulationsContext.Simulations.Where(s => s.SimId == simId)
            .Select(s => s.Data)
            .Single();
        return new JsonResult(simulations);
    }
}