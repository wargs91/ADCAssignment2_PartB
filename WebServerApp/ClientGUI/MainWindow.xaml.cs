using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using IronPython.Hosting;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<PythonCodeObj> NewCodeTaskList = new List<PythonCodeObj>();
        int JobsComplete;
        bool ActiveJob;
        bool JobWaiting;
        public MainWindow()
        {
            InitializeComponent();
            //Networking Thread
            //GET Client List from WebServerApp
            //while ActiveJob== False
                //Loop thorugh the list of IP addresses/Ports
                //connect and query for job
                    //if job exists
                        //run code using ironPython
                        //POST solution to the client
                       
                        //JobComnplete++
                        //Post so webserver (Why Do I need to do this)

            //Server Thread
                //Allow client to ask for job
                //POST next job on list
                //on return save result and chand Complete to true

        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "text files |*.txt";
            bool? response = openFileDialog.ShowDialog();



            if(response == true)
            {
                string filepath = openFileDialog.FileName;

                MessageBox.Show(filepath);

                if(System.IO.File.Exists(filepath)== true)
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
            
        }

        private void NetworkStatusButton_Click(object sender, RoutedEventArgs e)
        {

        }
        
    
    
    }
}
    