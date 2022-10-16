using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace ClientGUI
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext =true)]
    internal class PythonImplementation : ServerInterface
    {
        PythonCodeObj ServerInterface.GetNextTask()
        {
            PythonCodeObj pythonCodeObj = new PythonCodeObj();
            return pythonCodeObj;
        }

        PythonCodeObj ServerInterface.CompleteTask(PythonCodeObj newTask)
        {
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            engine.Execute(newTask.PyCodeBlock, scope);
            dynamic pythonFunction = scope.GetVariable(newTask.PyFunName);
            var result = pythonFunction();//need too figure out how to modify this for use
            newTask.Completed = true;
            return newTask;
        }
    }
}
