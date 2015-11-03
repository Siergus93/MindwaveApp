using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using MindwaveLib;
using NeuroSky.ThinkGear;
using System.Windows.Threading;
using System.IO;

namespace PuzzleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Connector connector;
        public static byte poorSig;

        public const int MaxWordNumber = 2709804;
        public static Random randomWordNumber;
        public string path = @"..\..\..\sjp-20151013\slowa-win-utf8.txt";

        public DataCollection attentionValuesCollection;

        public MainWindow()
        {
            InitializeComponent();

            attentionValuesCollection = new DataCollection();

            var attentionDataSource = new EnumerableDataSource<DataValue>(attentionValuesCollection);
            attentionDataSource.SetXMapping(x => dateAttention.ConvertToDouble(x.Date));
            attentionDataSource.SetYMapping(y => y.Value);
            plotterAttention.AddLineGraph(attentionDataSource, Colors.Red, 2, "Attetion");
            

            connector = new Connector();
            connector.DeviceConnected += new EventHandler(OnDeviceConnected);
            connector.DeviceConnectFail += new EventHandler(OnDeviceFail);
            connector.DeviceValidating += new EventHandler(OnDeviceValidating);

            connector.ConnectScan("COM3");

            //Stworzenie obiektu typu Random do wyboru losowego słowa ze słownika.
            randomWordNumber = new Random();

            
            

        }

        public string GetAWord()
        {
            int wordNumber = randomWordNumber.Next(0, MaxWordNumber);

            return ReadLine(path, wordNumber);
     
        }


        //http://stackoverflow.com/questions/1262965/how-do-i-read-a-specified-line-in-a-text-file
        public string ReadLine(string FilePath, int LineNumber)
        {
            string result = "";
            try
            {
                if (File.Exists(FilePath))
                {
                    using (StreamReader _StreamReader = new StreamReader(FilePath, System.Text.Encoding.UTF8, true))
                    {
                        for (int a = 0; a < LineNumber; a++)
                        {
                            result = _StreamReader.ReadLine();
                        }
                    }
                }
            }
            catch {
                
            }
            return result;
        }
         


        #region DeviceMethods

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            connector.Close();
            Environment.Exit(0);
        }


        public void OnDeviceConnected(object sender, EventArgs e)
        {


            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {

                portTextBlock.Text = "Device found on: " + de.Device.PortName;
            }));
            de.Device.DataReceived += new EventHandler(OnDataReceived);


        }

        // Called when scanning fails
        public void OnDeviceFail(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                portTextBlock.Text = "No devices found! :( ";
            }));
        }



        // Called when each port is being validated

        public void OnDeviceValidating(object sender, EventArgs e)
        {

            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                validatingTextBlock.Text += "Vali: ";
            }));

        }

        public void OnDataReceived(object sender, EventArgs e)
        {


            Device.DataEventArgs de = (Device.DataEventArgs)e;
            DataRow[] tempDataRowArray = de.DataRowArray;

            TGParser tgParser = new TGParser();
            tgParser.Read(de.DataRowArray);



            /* Loops through the newly parsed data of the connected headset*/
            // The comments below indicate and can be used to print out the different data outputs. 

            for (int i = 0; i < tgParser.ParsedData.Length; i++)
            {


                if (tgParser.ParsedData[i].ContainsKey("PoorSignal"))
                {

                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        poorSignalTextBlock.Text = "Poor Signal:" + tgParser.ParsedData[i]["PoorSignal"];

                    }));

                }


                if (tgParser.ParsedData[i].ContainsKey("Attention"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        attentionTextBlock.Text = "Att Value:" + tgParser.ParsedData[i]["Attention"];
                        Attention.Text = tgParser.ParsedData[i]["Attention"].ToString();
                        attentionValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["Attention"], DateTime.Now));
                    }));

                }

            }

        }
        #endregion

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

            WordLabel.Content = Reverse(GetAWord()).ToUpper(); 
        }

        //Source : http://stackoverflow.com/questions/228038/best-way-to-reverse-a-string
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }



    }
}
