using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ClientGUI
{
    public class NetworkStatus
    {
        public int Id { get; set; }
        public string IPAddress { get; set; }
        public string Port { get; set; }
        public Nullable<int> JobsCompleted { get; set; }
        public string status { get; set; }
    }

}
