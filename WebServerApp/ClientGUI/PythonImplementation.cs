using System;
using System.Text;
using System.ServiceModel;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
 using System.Security.Cryptography;
using RestSharp;
using Newtonsoft.Json;
using System.Runtime.Remoting;


namespace ClientGUI
{


    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
    internal class PythonImplementation : ServerInterface
    {
        
        public static PythonCodeObj pythonJob ;

        public static SHA256 sha256Hash = SHA256.Create();
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
       
        NetworkStatus ServerInterface.PutNetworkStatus(NetworkStatus status)
        {
            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/NetworkStatusTables/{id}", Method.Put);
            restRequest.AddUrlSegment("id", status.Id);
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

        PythonCodeObj ServerInterface.CompleteTask(PythonCodeObj postedNewTask)
        {

            string checkData = postedNewTask.PyCodeBlock;
            byte[] checkHash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(checkData));
            bool byteArrayComparison = ByteArrayCompare(checkHash, postedNewTask.codeHash);
            postedNewTask.PyFunName = "newJob";
            if(byteArrayComparison == true)
            {
                try
                {
                    string codeBlock = Decode(postedNewTask.PyCodeBlock);
                    ScriptEngine engine = Python.CreateEngine();
                    ScriptScope scope = engine.CreateScope();
                    engine.Execute(codeBlock, scope);
                    dynamic pythonFunction = scope.GetVariable(postedNewTask.PyFunName);
                    var result = pythonFunction();//need too figure out how to modify this for use
                    postedNewTask.Completed = true;
                    pythonJob = postedNewTask;
                    return postedNewTask;
                    
                }
                catch(Microsoft.Scripting.SyntaxErrorException)
                {
                    postedNewTask.Completed = true;
                    postedNewTask.result = "invalid syntax value, could not complete";
                    pythonJob = postedNewTask;
                    return postedNewTask;
                }
                
            }
            else
            {
               return null;
            }
        }

     static bool ByteArrayCompare(byte[] array1, byte[] array2)
        {
            if(array1.Length != array2.Length)
            {
                return false;
            }
            for(int i =0; i<array1.Length; i++)
                if (array1[i] != array2[i])
                    return false;

            return true;
        }
            
        
    }
}
