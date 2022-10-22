using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using RestSharp;
using WebServerApp.Models;
using IronPython.Compiler;
using System.ServiceModel;
using IronPython.Hosting;
using IronPython.Modules;
using IronPython;
using Microsoft.Scripting.Hosting;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<PythonCodeObj> NewCodeTaskList = new List<PythonCodeObj>();
        
        int JobsComplete = 0;
        public bool ActiveJob = false;
        string serverClientPort; 
        string serverClientIP;
        int serverClientID;
        private ServerInterface foob;
        bool disconnect = false;
        
        public MainWindow()
        {
            InitializeComponent();
            
            Task networkingTask = new Task(Networking);
            networkingTask.Start();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "text files |*.txt";
            bool? response = openFileDialog.ShowDialog();



            if (response == true)
            {
                string filepath = openFileDialog.FileName;

                MessageBox.Show(filepath);

                if (System.IO.File.Exists(filepath) == true)
                {
                    System.IO.StreamReader objReader;
                    objReader = new System.IO.StreamReader(filepath);
                    PythonInput.Text = objReader.ReadToEnd();
                    objReader.Close();
                }
                else
                {
                    MessageBox.Show("File Does Not Exist");
                }
            }
        }

        public void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            PythonCodeObj NewCodeTask = new PythonCodeObj();
            NewCodeTask.id = DateTime.Now.ToString();
            NewCodeTask.PyCodeBlock = PythonInput.Text;
            NewCodeTask.Completed = false;
            NewCodeTaskList.Add(NewCodeTask);
            ChannelFactory<ServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            //Set the URL and create the connection!
            
            string url = "net.tcp://" + serverClientIP + ":" + serverClientPort;
            foobFactory = new ChannelFactory<ServerInterface>(tcp, url);
            foob = foobFactory.CreateChannel();
            foob.PostNewJob(NewCodeTask);

            MessageBox.Show("Python Code submitted as job");

        }

        private void ShowJobsButton_Click(object sender, RoutedEventArgs e)
        {
            NetworkStatus networkStatus = new NetworkStatus();
            string status;
            if( ActiveJob == true)
            {
                status = "is currently working on a job";
            }
            else 
            {
                status = "is not currently working on a job";
            }
            networkStatus.status = status;
            networkStatus.jobComplete = JobsComplete;
            networkStatus.clientPort = serverClientPort;
            networkStatus.clientIP = serverClientIP;
            string output = "The system has completed " + JobsComplete + " jobs and " + status;
            NetworkStatusDisplay.Text = output;
        }
           

        private void SubmitClientButton_Click(object sender, RoutedEventArgs e)
        {
            
            UserRegistry newClient = new UserRegistry();
            newClient.Id = Int32.Parse(ID.Text);
            newClient.Port = ClientPort.Text;
            newClient.IPAddress = ClientIP.Text;
            serverClientIP = ClientIP.Text;
            serverClientPort = ClientPort.Text;
            serverClientID = Int32.Parse(ID.Text);

            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/UserRegistries", Method.Post);
            restRequest.AddJsonBody(JsonConvert.SerializeObject(newClient));
            RestResponse restResponse = restClient.Execute(restRequest);
            UserRegistry returnUser = JsonConvert.DeserializeObject<UserRegistry>(restResponse.Content);
            if (returnUser != null)
            {
                MessageBox.Show("Client Registered");
            }
            Task serverTask = new Task(Server);
            serverTask.Start();

        }

        public void Networking()
        {
            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/UserRegistries", Method.Get);
            RestResponse restResponse = restClient.Get(restRequest);
            List<UserRegistry> userRegistries = JsonConvert.DeserializeObject<List<UserRegistry>>(restResponse.Content);
            Random rnd = new Random();
            int length = userRegistries.Count;
            Random r = new Random(length);

            while (ActiveJob == false)
            {
                foreach (int i in Enumerable.Range(0,length).OrderBy(x => r.Next()))
                {
                    PythonCodeObj nextTask;
                    ChannelFactory<ServerInterface> foobFactory;
                    NetTcpBinding tcp = new NetTcpBinding();
                    //Set the URL and create the connection!
                    string IPAddress = userRegistries[i].IPAddress;
                    string Port = userRegistries[i].Port;
                    string url = "net.tcp://" + IPAddress + ":" + Port;
                    try
                    {
                        foobFactory = new ChannelFactory<ServerInterface>(tcp, url);
                        foob = foobFactory.CreateChannel();
                        nextTask = foob.GetNextTask();
                        if (nextTask != null)
                        {
                            ActiveJob = true;
                            foob.CompleteTask(nextTask);
                            ActiveJob = false;
                        }
                    }
                    catch (EndpointNotFoundException)
                    {
                        //do nothing
                    }

                }
            }
        }

        public void Server()
        {
            while (disconnect == false)
            {
                ServiceHost host;
                NetTcpBinding tcp = new NetTcpBinding();
                host = new ServiceHost(typeof(PythonImplementation));
                string url = "net.tcp://" + serverClientIP + ":" + serverClientPort;
                host.AddServiceEndpoint(typeof(ServerInterface), tcp, url);
                try
                {
                    host.Open();
                    //need to post next task here
                    while (disconnect == false)
                    {
                        Thread.Sleep(10000);
                    }
                    host.Close();
                }
                catch (AddressAlreadyInUseException)
                {
                    MessageBox.Show("Address Already in Use");
                    ClientPort.Text = "";
                    ClientIP.Text = "";
                }
               
            }           
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            disconnect = true;
            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/UserRegistries/{id}", Method.Delete);
            restRequest.AddUrlSegment("id", serverClientID);
            RestResponse restResponse = restClient.Execute(restRequest);
        }
    }
}


    