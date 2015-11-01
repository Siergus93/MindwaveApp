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

namespace PuzzleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Connector connector;
        public static byte poorSig;


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
        }

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

            //Device d = (Device)sender;

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
                        attentionValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["Attention"], DateTime.Now));
                    }));

                }

            }

        }



    }
}
