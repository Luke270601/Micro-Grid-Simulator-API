using ActressMas;

namespace Micro_Grid_Management.Micro_Grid
{
    public class BatteryAgent : Agent
    {
        private double supply = 0;
        private double supplyDemand = 0;
        
        public override void Setup()
        {
            supply = 0;
        }

        public override void Act(Message message)
        {
            try
            {
                Settings.Packet packet = new Settings.Packet();
                message.Parse(out string action, out string parameters);

                switch (action)
                {
                    case "supply":
                        supplyDemand = Convert.ToDouble(parameters);
                        supply += supplyDemand;
                        Console.WriteLine("Currently Stored: " + supply);
                        packet = new Settings.Packet("Battery", "Stored: " + supply);
                        Settings.Packets.Add(packet);
                        Send("GridManager", "energy_stored");
                        break;
                    
                    case "request":
                        supplyDemand = Convert.ToDouble(parameters);
                        if (supply < supplyDemand)
                        {
                            supplyDemand -= supply;
                            Send("GridManager", $"demand_remaining {supplyDemand}");
                        }

                        else
                        {
                            supply -= supplyDemand;
                            Send("GridManager", "demand_met");
                        }
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

        public override void ActDefault()
        {
            
        }
    }
}