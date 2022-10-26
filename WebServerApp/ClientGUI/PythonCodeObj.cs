using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientGUI
{
    public class PythonCodeObj : MarshalByRefObject
    {
        public string id { get; set; }
        public string PyCodeBlock{ get; set; }
        public string result { get; set; }
        public bool Completed { get; set; }
        public string PyFunName { get; set; }
        public byte[] codeHash { get; set; }  


    }

    
}
