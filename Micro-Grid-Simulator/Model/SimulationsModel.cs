using System.ComponentModel.DataAnnotations;

namespace Micro_Grid_Simulator.Model;

public class SimulationsModel
{
    [Key]
    public int SimId { get; set; }
    public string Date { get; set; }
    public string Data { get; set; }
    public int TurbineCount { get; set; }
    public int PanelCount { get; set; }
    public int HouseCount { get; set; }
    
    public int Duration { get; set; }
}