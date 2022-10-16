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

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<PythonCodeObj> NewCodeTaskList = new List<PythonCodeObj>();
        int JobsComplete = 0;
        public bool ActiveJob;
        bool JobWaiting;
        public MainWindow()
        {
            InitializeComponent();
            while (ActiveJob == false)
            {
                Networking();//delegate or asynch
            }
            Server();
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
            string status;
            if( ActiveJob == true)
            {
                status = "is currently working on a job";
            }
            else 
            {
                status = "is not currently working on a job";
            }
            string output = "The system has completed " + JobsComplete + " jobs and " + status;
            NetworkStatusDisplay.Text = output;

        }

        private void NetworkStatusButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void Networking()
        {

           
        }


        public void Server()
        {

        }

    }
}


    