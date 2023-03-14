using System.Text.Json;
using ActressMas;
using Micro_Grid_Management.Micro_Grid;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Micro_Grid_Simulator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Simulation : ControllerBase
{
    [HttpGet]
    public JsonResult RunSim(int duration, int turbineCount, int panelCount, int houseCount, string monthOfTheYear)
    {
        Settings.DaysRunning = duration;
        Settings.TurbineCount = turbineCount;
        Settings.PanelCount = panelCount;
        Settings.HouseCount = houseCount;
        Settings.MonthOfTheYear = monthOfTheYear;
        // inefficient, uses broadcast to simulate public open-cry auction

        //Sets up the ActressMas environment agent and the agent used to inform house agents
        var env = new EnvironmentMas(randomOrder: false, parallel: false);

        //Creates and instance of the energy distribution, battery agent and grid management in the environment
        var gridManager = new GridManager();
        var batteryAgent = new BatteryAgent();
        var environmentAgent = new EnvironmentAgent();

        env.Add(gridManager, "GridManager");
        env.Add(batteryAgent, "BatteryStorage");
        env.Add(environmentAgent, "Environment");

        //Adds houses to environment 
        for (int i = 1; i <= Settings.HouseCount; i++)
        {
            var houseAgent = new HouseAgent();
            env.Add(houseAgent, $"houseAgent{i:D2}");
        }

        //Adds solar panels to environment 
        for (int i = 1; i <= Settings.PanelCount; i++)
        {
            var solarPanelAgent = new SolarPanelAgent();
            env.Add(solarPanelAgent, $"solarPanel{i:D2}");
        }

        //Adds wind turbines to environment 
        for (int i = 1; i <= Settings.TurbineCount; i++)
        {
            var turbineAgent = new WindTurbineAgent();
            env.Add(turbineAgent, $"turbine{i:D2}");
        }

        env.Start();
        var json = JsonSerializer.Serialize(Settings.Packets);
        return new JsonResult(json);
    }
}