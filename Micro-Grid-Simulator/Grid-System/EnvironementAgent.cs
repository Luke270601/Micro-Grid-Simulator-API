using ActressMas;

namespace Micro_Grid_Management.Micro_Grid
{
    class EnvironmentAgent : Agent
    {
        private string _senderID = "";
        private string _content = "";
        private double _demand = 0;
        private double _solarRadiation = 0;
        private double _windSpeed = 0;
        
        private List<double> _windRecords = new List<double>();
        private List<double> _solarRecords = new List<double>();
        private List<double> _houseDemand = new List<double>();
    public override void Setup()
    {
        _solarRecords = SolarListSetup();
        _windRecords = WindListSetup();
    }

    public override void Act(Message message)
    {
        string content = "";
        switch (message.Content)
        {
            case "solar": //this agent only responds to "start" messages
                _senderID = message.Sender;
                _solarRadiation = _solarRecords[Settings.HoursRunning];
                
                content = $"inform {_solarRadiation}";
                Send(_senderID, content); //send the message with this information back to the household agent that requested it
                break;
            
            case "wind": //this agent only responds to "start" messages
                _senderID = message.Sender; //get the sender's name so we can reply to them
                _windSpeed = _windRecords[Settings.HoursRunning];
                
                content = $"inform {_windSpeed}";
                Send(_senderID, content); //send the message with this information back to the household agent that requested it
                break;
            
            case "house": //this agent only responds to "start" messages
                _senderID = message.Sender; //get the sender's name so we can reply to them
                _demand = 0;
                
                content = $"inform {_demand}";
                Send(_senderID, content); //send the message with this information back to the household agent that requested it
                break;
            
            case "stop":
                Stop();
                break;

            default:
                break;
        }
    }

    private List<double> SolarListSetup()
    {
        using (var reader = new StreamReader(@"C:\Users\lukes\RiderProjects\Micro-Grid Energy Management\Micro-Grid Energy Management\TestData\SolarData.csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                _solarRecords.Add(Convert.ToDouble(values[0]));
            }
        }
        _solarRecords.Add(0);

        return _solarRecords;
    }
    
    private List<double> WindListSetup()
    {
        using (var reader = new StreamReader(@"C:\Users\lukes\RiderProjects\Micro-Grid Energy Management\Micro-Grid Energy Management\TestData\WindData.csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                _windRecords.Add(Convert.ToDouble(values[0]));
            }
        }
        _windRecords.Add(0);

        return _windRecords;
    }
    
    private void DemandListSetup()
    {
        string filePath = "";
        string[] list = File.ReadAllLines(filePath);

        foreach (var item in list)
        {
            _houseDemand.Add(Convert.ToDouble(item));  
        }
    }
}
}