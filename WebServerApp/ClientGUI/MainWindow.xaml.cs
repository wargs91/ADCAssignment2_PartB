using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using RestSharp;
using WebServerApp.Models;
using IronPython;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<PythonCodeObj> NewCodeTaskList = new List<PythonCodeObj>();
        int JobsComplete = 0;
        bool ActiveJob;
        bool JobWaiting;
        public MainWindow()
        {
            InitializeComponent();
            //delegate netwroking task
            //delegate server thread



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

        }

        private void ShowJobsButton_Click(object sender, RoutedEventArgs e)
        {
            //create string showing number of jobs complete and if a job is currently being processed
        }

        private void NetworkStatusButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void Networking()
        {
            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/UserRegistries", Method.Get);
            RestResponse restResponse = restClient.Get(restRequest);

            List<UserRegistry> userRegistries = JsonConvert.DeserializeObject<List<UserRegistry>>(restResponse.Content);
            while (ActiveJob == false)
            {
                //Loop thorugh the list of IP addresses/Ports
                //connect and query for job
                if (JobWaiting == true)
                {
                    //use Iron Python to execute code
                    // return result to client
                    JobsComplete++;
                    //Post to webserver?
                }

            }

        }
        public void Server()
        { 
          //Server Thread
          //Allow client to ask for job
          //POST next job on list
          //on return save result and chand Complete to true

        }
    }
}
    