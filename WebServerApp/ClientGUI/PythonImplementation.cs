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
using RestSharp;
using Newtonsoft.Json;


namespace ClientGUI
{
    

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
    internal class PythonImplementation : ServerInterface
    {
        
        public PythonCodeObj pythonJob;//This keeps returning to null and then cannot pull the python Job
        SHA256 sha256Hash = SHA256.Create();
        PythonCodeObj ServerInterface.PostNewJob(PythonCodeObj newJob)
        {
            newJob.PyCodeBlock = Encode(newJob.PyCodeBlock);
            string data = newJob.PyCodeBlock;
            byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            newJob.codeHash = hash;
            
            pythonJob = newJob;
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
            if (String.IsNullOrEmpty(code))
            {
                return code;
            }
            byte[] textBytes = Encoding.UTF8.GetBytes(code);
            return Convert.ToBase64String(textBytes);
        }
        NetworkStatus ServerInterface.PostNetworkStatus(NetworkStatus status)
        {
            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/NetworkStatus", Method.Post);
            restRequest.AddJsonBody(JsonConvert.SerializeObject(status));
            RestResponse restResponse = restClient.Execute(restRequest);

            return status;
        }

        PythonCodeObj ServerInterface.GetNextTask()
        {
            
            
            if (pythonJob != null)
            { 
            return pythonJob;
                
            }                  
            else 
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

       
            
        
    }
}
