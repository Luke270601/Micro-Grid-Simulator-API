using ActressMas;

namespace Micro_Grid_Management.Micro_Grid
{
    public class GridManager : Agent
    {
        private int _houseAgentCount = Settings.HouseCount;
        private int _solarPanelCount = Settings.PanelCount;
        private int _windTurbineCount = Settings.TurbineCount;
        private double _supply = 0;
        private double _demand = 0;
        private int _hoursRemaining = Settings.HoursRunning;
        private bool _finished = false;
        private bool _tookFromGrid = false;
        private Settings.Packet _packet = new Settings.Packet();


        public override void Setup()
        {
        }

        public override void Act(Message message)
        {
            try
            {
                message.Parse(out var action, out string parameters);
                if (Settings.HoursRunning < Settings.DaysRunning * 24)
                {
                    switch (action)
                    {
                        case "demand":
                            Console.WriteLine(message.Sender + " : " + parameters);
                            _packet = new Settings.Packet(message.Sender, parameters);
                            Settings.Packets.Add(_packet);
                            _demand += Convert.ToDouble(parameters);
                            _houseAgentCount--;
                            break;

                        case "supply":
                            Console.WriteLine(message.Sender + " : " + parameters);
                            _packet = new Settings.Packet(message.Sender, parameters);
                            Settings.Packets.Add(_packet);
                            if (message.Sender.Contains("solarPanel"))
                            {
                                _solarPanelCount--;
                                _supply += Convert.ToDouble(parameters);
                            }

                            else
                            {
                                _windTurbineCount--;
                                _supply += Convert.ToDouble(parameters);
                            }

                            break;

                        case "energy_stored":
                            Console.WriteLine(message.Sender + " : " + "Energy has been stored");
                            _packet = new Settings.Packet(message.Sender, "Energy has been stored");
                            Settings.Packets.Add(_packet);
                            Broadcast("generate");
                            Settings.HoursRunning++;
                            break;

                        case "demand_met":
                            Console.WriteLine(message.Sender + " : " + "Battery provided power");
                            _packet = new Settings.Packet(message.Sender, "Battery provided power");
                            Settings.Packets.Add(_packet);
                            Broadcast("generate");
                            Settings.HoursRunning++;
                            break;

                        case "demand_remaining":
                            _demand = Convert.ToDouble(parameters);
                            Console.WriteLine(message.Sender + " : " + Convert.ToDouble(parameters) +
                                              " demand remaining");
                            _packet = new Settings.Packet(message.Sender, parameters);
                            Settings.Packets.Add(_packet);
                            SupplyFromGrid(_demand);
                            Broadcast("generate");
                            Settings.HoursRunning++;
                            break;

                        case "stop":
                            Stop();
                            break;
                    }
                }
                else
                {
                    _tookFromGrid = false;
                    Send(message.Sender, "stop");
                    _finished = true;
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
            if (_houseAgentCount == 0 && _solarPanelCount == 0 && _windTurbineCount == 0)
            {
                _houseAgentCount = Settings.HouseCount;
                _solarPanelCount = Settings.PanelCount;
                _windTurbineCount = Settings.TurbineCount;
                SupplyDemand(_supply, _demand);
                _supply = 0;
                _demand = 0;
            }
            else if (_finished)
            {
                if (!_tookFromGrid)
                {
                    Console.WriteLine("End of simulation");
                    Console.WriteLine("Total From Grid: " + Settings.EnergyFromGrid + " kw/h");
                    _packet = new Settings.Packet("Grid", Settings.EnergyFromGrid + " kw/h");
                    Settings.Packets.Add(_packet);
                }

                _finished = false;
                Settings.HoursRunning = _hoursRemaining;
                Send("BatteryStorage", "stop");
                Send("Environment", "stop");
                Stop();
            }
        }

        private void SupplyDemand(double supply, double demand)
        {
            if (supply > demand)
            {
                supply -= demand;
                Send("BatteryStorage", $"supply {supply}");
            }

            else if (supply < demand)
            {
                demand -= supply;
                Send("BatteryStorage", $"request {demand}");
            }

            else
            {
                Broadcast("generate");
            }
        }

        private void SupplyFromGrid(double demand)
        {
            _tookFromGrid = true;
            Settings.EnergyFromGrid += demand;
            Console.WriteLine("Taken From Grid: " + demand);
            _packet = new Settings.Packet("Grid", "Taken From Grid: " + demand + " kw/h");
            Settings.Packets.Add(_packet);
            Console.WriteLine("Total From Grid: " + Settings.EnergyFromGrid);
            _packet = new Settings.Packet("Grid", "Total From Grid: " + Settings.EnergyFromGrid + " kw/h");
            Settings.Packets.Add(_packet);
        }
    }
}