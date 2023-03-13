using System.Text.Json;
using Micro_Grid_Management.Micro_Grid;
using Microsoft.AspNetCore.Mvc;

namespace Micro_Grid_Simulator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GetSimData : ControllerBase
{
    [HttpGet("GetNames")]
    public List<string> GetDataCount()
    {
        List<string> list = new List<string>();

        for (int i = 0; i < 10; i++)
        {
            list.Add("Simulation " + (i+1));
        }

        return list;
    }
    
    [HttpGet("GetData/{name}")]
    public JsonResult GetSimInfo(string name)
    {
        var json = JsonSerializer.Serialize("Dog: dog");
        return new JsonResult(json);
    }
}