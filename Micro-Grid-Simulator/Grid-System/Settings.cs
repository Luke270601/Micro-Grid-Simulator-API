using System.Collections.Specialized;
using System.Runtime.InteropServices.JavaScript;

namespace Micro_Grid_Management.Micro_Grid
{
    public class Settings
    {
        public static int PanelCount = 1;
        public static int TurbineCount = 3;
        public static int HouseCount = 10;
        public static double EnergyFromGrid = 0;
        public static int HoursRunning = 0;
        public static int DaysRunning = 0;

        public struct Packet
        {
            public string Sender { get; set; }
            public string Message { get; set; }

            public Packet(string sender, string message)
            {
                Sender = sender;
                Message = message;
            }
        }

        public static List<Packet> Packets = new List<Packet>();
        
        public static Random RandomDemand = new Random();

        //Wind Turbine Information
        //radius of turbine rotor
        public static int radius = 58;
        public static double airDensity = 1.2;
        //how well wind can be converted to power
        public static double efficiencyFactor= 0.5;
        
        //Solar Panel Information
        //area of the solar panels
        public static double area = 3.12;
        //how efficiently sunlight is converted to energy 
        public static double efficiency = 0.2;
        //the solar iradtion in kw/h m^2
        public static double performanceRatio = 0.75;
        
    }
}