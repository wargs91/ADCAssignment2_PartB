
using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using RestSharp;
using WebServerApp.Models;
using System.ServiceModel;
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
        PythonCodeObj NewCodeTask = new PythonCodeObj();
        
        public int JobsComplete = 0;
        public bool ActiveJob = false;
        public string serverClientPort; 
        public string serverClientIP;
        public int serverClientID;
        private ServerInterface foob;
        public bool disconnect = false;
        public NetworkStatus networkStatus = new NetworkStatus();
        public string output = "wating for jobs";
        public string resultOutput;

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
            
            NewCodeTask.id = DateTime.Now.ToString();
            NewCodeTask.PyCodeBlock = PythonInput.Text;
            NewCodeTask.Completed = false;
            
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

            networkStatus.Id = serverClientID;
            networkStatus.IPAddress = serverClientIP;
            networkStatus.Port = serverClientPort;
            networkStatus.status = "Connected and waiting for jobs";
            networkStatus.JobsCompleted = JobsComplete;

            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/UserRegistries", Method.Post);
            restRequest.AddJsonBody(JsonConvert.SerializeObject(newClient));
            RestResponse restResponse = restClient.Execute(restRequest);
            UserRegistry returnUser = JsonConvert.DeserializeObject<UserRegistry>(restResponse.Content);

            RestClient statusRestClient = new RestClient("http://localhost:49901/");
            RestRequest statusRestRequest = new RestRequest("api/NetworkStatusTables", Method.Post);
            statusRestRequest.AddJsonBody(JsonConvert.SerializeObject(networkStatus));
            RestResponse statusRestResponse = statusRestClient.Execute(statusRestRequest);
            NetworkStatusTable returnNetworkStatus = JsonConvert.DeserializeObject<NetworkStatusTable>(statusRestResponse.Content);

            if (returnUser != null && returnNetworkStatus != null)
            {
                MessageBox.Show("Client Registered");
            }
            else
            {
                MessageBox.Show("An Error Occurred, Client Not Reqistered");
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
                foreach (int i in Enumerable.Range(0, length).OrderBy(x => r.Next()))
                {
                    PythonCodeObj nextTask;
                    ChannelFactory<ServerInterface> foobFactory;
                    NetTcpBinding tcp = new NetTcpBinding();
                    //Set the URL and create the connection!tha
                    
                    string IPAddress = userRegistries[i].IPAddress;
                    string Port = userRegistries[i].Port;
                    string url = ("net.tcp://" + IPAddress + ":" + Port);
                    try
                    {
                        foobFactory = new ChannelFactory<ServerInterface>(tcp, url);
                        foob = foobFactory.CreateChannel();
                        nextTask = foob.GetNextTask();
                        if (nextTask != null)
                        {
                            if (nextTask.Completed == false && nextTask.id != NewCodeTask.id)
                            {
                                ActiveJob = true;
                                networkStatus.status = "Currently working on a job";
                                UpdateNetworkDisplay(networkStatus.status);
                                networkStatus.Id = serverClientID;
                                foob.PutNetworkStatus(networkStatus);
                                nextTask = foob.CompleteTask(nextTask);
                                networkStatus.JobsCompleted++;
                                networkStatus.status = "Not currently working on a job\n"+networkStatus.JobsCompleted+" Jobs Completed";
                                UpdateNetworkDisplay(networkStatus.status);
                                UpdateDisplay(nextTask.result);
                                                                
                                foob.PutNetworkStatus(networkStatus);
                                ActiveJob = false;
                            }
                            if (nextTask.Completed == true && nextTask.id == NewCodeTask.id)
                            {
                                resultOutput = "Computation completed remotely.\nResult:\n" + nextTask.result;
                                UpdateDisplay(resultOutput);
                            }
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
                string serverUrl = ("net.tcp://" + serverClientIP + ":" + serverClientPort);
                host.AddServiceEndpoint(typeof(ServerInterface), tcp, serverUrl);
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
        public void UpdateDisplay(string input)
        {
            this.Dispatcher.Invoke(() =>
            {
                Results.Text = input;
            });
        }

        public void UpdateNetworkDisplay(string input)
        {
            this.Dispatcher.Invoke(() =>
            {
                NetworkStatusDisplay.Text = input;
            });
        }
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            disconnect = true;
            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/UserRegistries/{id}", Method.Delete);
            restRequest.AddUrlSegment("id", serverClientID);
            RestResponse restResponse = restClient.Execute(restRequest);

            MessageBox.Show("Client Disconnected");

        }
                  
    }
}


    