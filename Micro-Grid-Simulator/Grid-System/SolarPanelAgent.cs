using ActressMas;

namespace Micro_Grid_Management.Micro_Grid
{
    public class SolarPanelAgent : Agent
    {
        private double _power = 0;
        private int noMessageCount = 0;
        private Random _irradiance = new Random();

        public override void Setup()
        {
            Send("Environment", "solar");
        }

        public override void Act(Message message)
        {
            noMessageCount = 0;
            try
            {
                message.Parse(out string action, out string parameters);
                switch (action)
                {
                    case "generate":
                        Send("Environment", "solar");
                        break;

                    case "inform":
                        GenerateOutput(Convert.ToDouble(parameters));
                        break;

                    case "stop":
                        Stop();
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void GenerateOutput(double solarRadiation)
        {
            _power = Math.Round(
                (Settings.area * Settings.efficiency * Settings.performanceRatio * solarRadiation) / 1000, 3);
            Send("GridManager", $"supply {_power}");
        }

        public override void ActDefault()
        {
            noMessageCount++;
            _power = 0;
            
            if (noMessageCount > 2)
            {
                Stop();
            }
        }
    }
}