using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Text;
using System.Security.Cryptography;


namespace ClientGUI
{

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
    internal class PythonImplementation : ServerInterface
    {
        List<PythonCodeObj> jobList = new List<PythonCodeObj>();
        SHA256 sha256Hash = SHA256.Create();
        PythonCodeObj ServerInterface.GetNextTask()
        {
            for (int i = 0; i < jobList.Count; i++)
            {
                if (jobList[i].Completed == false)
                    return jobList[i];
                break;
            }
            return null;
        }

        PythonCodeObj ServerInterface.CompleteTask(PythonCodeObj newTask)
        {

            string checkData = newTask.PyCodeBlock;
            byte[] checkHash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(checkData));
            if(newTask.codeHash == checkHash)
            {
                string codeBlock = Decode(newTask.PyCodeBlock);
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                engine.Execute(codeBlock, scope);
                dynamic pythonFunction = scope.GetVariable(newTask.PyFunName);
                var result = pythonFunction();//need too figure out how to modify this for use
                newTask.Completed = true;
                return newTask;
            }
            else
            {
                return null;
            }
        }

        PythonCodeObj ServerInterface.PostNextJob(PythonCodeObj newJob)
        {
            newJob.PyCodeBlock = Encode(newJob.PyCodeBlock);
            string data = newJob.PyCodeBlock;
            byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            newJob.codeHash = hash;
            jobList.Add(newJob);
            return newJob;
        }

        string Decode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return code;
            }
            byte[] encodedBytes = Convert.FromBase64String(code);
            return Encoding.UTF8.GetString(encodedBytes);
        }
        
        string Encode(string code)
        {
            if(String.IsNullOrEmpty(code))
            {
                return code;
            }
            byte[] textBytes = Encoding.UTF8.GetBytes(code);
            return Convert.ToBase64String(textBytes);
        }
       
        
            
        
    }
}
