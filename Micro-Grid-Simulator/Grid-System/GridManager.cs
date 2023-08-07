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
                if (Settings.HoursRunning < 24 && Settings.DaysDone < Settings.DaysRunning)
                {
                    switch (action)
                    {
                        case "demand":
                            _packet = new Settings.Packet(message.Sender, parameters);
                            Settings.Packets.Add(_packet);
                            _demand += Convert.ToDouble(parameters);
                            _houseAgentCount--;
                            break;

                        case "supply":
                            if (message.Sender.Contains("solarPanel"))
                            {
                                _solarPanelCount--;
                                _supply += Convert.ToDouble(parameters);
                                _packet = new Settings.Packet(message.Sender, parameters);
                                Settings.Packets.Add(_packet);
                            }

                            else
                            {
                                _windTurbineCount--;
                                _supply += Convert.ToDouble(parameters);
                                _packet = new Settings.Packet(message.Sender, parameters);
                                Settings.Packets.Add(_packet);
                            }

                            break;

                        case "energy_stored":
                            Broadcast("generate");
                            Settings.HoursRunning++;
                            break;

                        case "demand_met":
                            Broadcast("generate");
                            Settings.HoursRunning++;
                            break;

                        case "demand_remaining":
                            _demand = Convert.ToDouble(parameters);
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
                Settings.DaysDone = 0;
                Send("BatteryStorage", "stop");
                Send("Environment", "stop");
                Console.WriteLine("Energy Supplied Total: " + Settings.SupplementedFromBattery);
                Settings.SupplementedFromBattery = 0;
                Settings.EnergyFromGrid = 0;
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
            _packet = new Settings.Packet("Grid", "Taken From Grid: " + demand + " kw/h");
            Settings.Packets.Add(_packet);
            _packet = new Settings.Packet("Grid", "Total From Grid: " + Settings.EnergyFromGrid + " kw/h");
            Settings.Packets.Add(_packet);
            _supply = 0;
            _demand = 0;
        }
    }
}