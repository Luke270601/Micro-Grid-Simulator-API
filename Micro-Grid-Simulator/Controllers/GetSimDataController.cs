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
        var products = await _simulationsContext.Simulations.ToListAsync();
        return Ok(products);
    }
    
    [HttpGet("GetData/{name}")]
    public JsonResult GetSimInfo(string name)
    {
        var json = JsonSerializer.Serialize("Dog: dog");
        return new JsonResult(json);
    }
}