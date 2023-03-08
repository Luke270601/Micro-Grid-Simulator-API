using ActressMas;

namespace Micro_Grid_Management.Micro_Grid
{
    public class HouseAgent : Agent
    {
        private double demand = 0;

        public override void Setup()
        {
            Send("Environment", "house");
        }

        public override void Act(Message message)
        {
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
            demand = Math.Round(Settings.RandomDemand.NextDouble() * (0.981 - 0.308) + 0.308, 3);
            Send("GridManager", $"demand {demand}");
        }


        public override void ActDefault()
        {
            demand = 0;
        }
    }
}