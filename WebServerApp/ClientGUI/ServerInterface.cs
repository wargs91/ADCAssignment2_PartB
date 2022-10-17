using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ClientGUI
{
    [ServiceContract]
    internal interface ServerInterface
    {
                
        [OperationContract]
        PythonCodeObj GetNextTask();
        [OperationContract]
        PythonCodeObj CompleteTask(PythonCodeObj newTask);
        [OperationContract]
        PythonCodeObj PostNextJob(PythonCodeObj newJob);

    }
}


