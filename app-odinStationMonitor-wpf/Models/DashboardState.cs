using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacoStationMonitor.Models
{
    public class DashboardState
    {
        public int PartID { get; set; }= 111;
        public int NextPartID { get; set; } = 112;
        public string ChildPartNo { get; set; }= "00547354600109";
        public string StatusString { get; set; } = "In Progress";
        public string MatNo { get; set; } = "MAT1234567890ABCD";
        public string StationName { get; set; }= "Station1";
        public bool IsError { get; set; }
    }
}
