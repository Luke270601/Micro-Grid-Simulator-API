using ActressMas;

namespace Micro_Grid_Management.Micro_Grid
{
    public class WindTurbineAgent : Agent
    {
        private double _power = 0;
        private Random _windSpeed = new Random();

        public override void Setup()
        {
            Send("Environment", "wind");
        }

        public override void Act(Message message)
        {
            try
            {
                message.Parse(out string action, out string parameters);
                switch (action)
                {
                    case "generate":
                        Send("Environment", "wind");
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

        private void GenerateOutput(double windSpeed)
        {
            //Formula P = π/2 * r² * v³ * ρ * η;
            //Wind speed is in m/s
            _power = Math.Round(
                (Math.PI / 2 * Math.Pow(Settings.radius, 2) * Math.Pow(windSpeed, 3) * Settings.airDensity *
                 Settings.efficiencyFactor) / 1000, 3);
            Send("GridManager", $"supply {_power}");
        }

        public override void ActDefault()
        {
            _power = 0;
        }
    }
}