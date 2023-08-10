using ActressMas;

namespace Micro_Grid_Management.Micro_Grid
{
    public class HouseAgent : Agent
    {
        private double demand = 0;
        private int noMessageCount = 0;
        private double dailyDemand = 0;

        private List<double> distribution = new() {0.95, 0.91, 0.9, 1.01, 2.03, 2.50, 5.01, 9.01, 10.50, 8.54, 7.01, 4.60, 4.01, 3.01, 2.01, 4.01, 8.99, 10.01, 5.99, 4.01, 1.99, 0.99, 1.01, 1.00};

        public override void Setup()
        {
            Send("Environment", "house");
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
                        Send("Environment", "house");
                        break;

                    case "inform":
                        GenerateDemand();
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

        private void GenerateDemand()
        {
            if (Settings.HoursRunning == 0)
            {
                dailyDemand = Settings.RandomDemand.NextDouble()*(10 - 8.5) + 8.5;
            }

            demand = Math.Round(dailyDemand / 100 * distribution[Settings.HoursRunning], 3);
            Send("GridManager", $"demand {demand}");
        }


        public override void ActDefault()
        {
            noMessageCount++;
            demand = 0;

            if (noMessageCount > 2)
            {
                Stop();
            }
        }
    }
}