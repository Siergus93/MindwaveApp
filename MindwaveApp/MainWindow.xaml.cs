using System;
using System.Windows;
using System.Windows.Media;
using NeuroSky.ThinkGear;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.ComponentModel;
using MindwaveLib;

namespace MindwaveApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Connector connector;
        public static byte poorSig;
        public DataCollection attentionValuesCollection;
        public DataCollection meditationValuesCollection;
        public DataCollection deltaValuesCollection;
        public DataCollection thetaValuesCollection;
        public DataCollection lowAlphaValuesCollection;
        public DataCollection highAlphaValuesCollection;
        public DataCollection lowBetaValuesCollection;
        public DataCollection highBetaValuesCollection;
        public DataCollection lowGammaValuesCollection;
        public DataCollection midGammaValuesCollection;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            attentionValuesCollection = new DataCollection();
            meditationValuesCollection = new DataCollection();

            deltaValuesCollection = new DataCollection();
            thetaValuesCollection = new DataCollection();

            lowAlphaValuesCollection = new DataCollection();
            highAlphaValuesCollection = new DataCollection();

            lowBetaValuesCollection = new DataCollection();
            highBetaValuesCollection = new DataCollection();

            lowGammaValuesCollection = new DataCollection();
            midGammaValuesCollection = new DataCollection();

            var attentionDataSource = new EnumerableDataSource<Data>(attentionValuesCollection);
            attentionDataSource.SetXMapping(x => dateAttention.ConvertToDouble(x.Date));
            attentionDataSource.SetYMapping(y => y.Value);
            plotterAttention.AddLineGraph(attentionDataSource, Colors.Red, 2, "Attention");

            var meditationDataSource = new EnumerableDataSource<Data>(meditationValuesCollection);
            meditationDataSource.SetXMapping(x => dateMeditation.ConvertToDouble(x.Date));
            meditationDataSource.SetYMapping(y => y.Value);
            plotterMeditation.AddLineGraph(meditationDataSource, Colors.Green, 2, "Meditation");

            var deltaDataSource = new EnumerableDataSource<Data>(deltaValuesCollection);
            deltaDataSource.SetXMapping(x => dateDelta.ConvertToDouble(x.Date));
            deltaDataSource.SetYMapping(y => y.Value);
            plotterDelta.AddLineGraph(deltaDataSource, Colors.Blue, 2, "Delta");

            var thetaDataSource = new EnumerableDataSource<Data>(thetaValuesCollection);
            thetaDataSource.SetXMapping(x => dateTheta.ConvertToDouble(x.Date));
            thetaDataSource.SetYMapping(y => y.Value);
            plotterTheta.AddLineGraph(thetaDataSource, Colors.Cyan, 2, "Theta");

            var lowAlphaDataSource = new EnumerableDataSource<Data>(lowAlphaValuesCollection);
            lowAlphaDataSource.SetXMapping(x => dateLowAlpha.ConvertToDouble(x.Date));
            lowAlphaDataSource.SetYMapping(y => y.Value);
            plotterLowAlpha.AddLineGraph(lowAlphaDataSource, Colors.Crimson, 2, "Low Alpha");

            var highAlphaDataSource = new EnumerableDataSource<Data>(highAlphaValuesCollection);
            highAlphaDataSource.SetXMapping(x => dateHighAlpha.ConvertToDouble(x.Date));
            highAlphaDataSource.SetYMapping(y => y.Value);
            plotterHighAlpha.AddLineGraph(highAlphaDataSource, Colors.Indigo, 2, "High Alpha");

            var lowBetaDataSource = new EnumerableDataSource<Data>(lowBetaValuesCollection);
            lowBetaDataSource.SetXMapping(x => dateTheta.ConvertToDouble(x.Date));
            lowBetaDataSource.SetYMapping(y => y.Value);
            plotterLowBeta.AddLineGraph(lowBetaDataSource, Colors.Orchid, 2, "Low Beta");

            var highBetaDataSource = new EnumerableDataSource<Data>(highBetaValuesCollection);
            highBetaDataSource.SetXMapping(x => dateTheta.ConvertToDouble(x.Date));
            highBetaDataSource.SetYMapping(y => y.Value);
            plotterHighBeta.AddLineGraph(highBetaDataSource, Colors.Blue, 2, "High Beta");

            var lowGammaDataSource = new EnumerableDataSource<Data>(lowGammaValuesCollection);
            lowGammaDataSource.SetXMapping(x => dateLowGamma.ConvertToDouble(x.Date));
            lowGammaDataSource.SetYMapping(y => y.Value);
            plotterLowGamma.AddLineGraph(lowGammaDataSource, Colors.Purple, 2, "Low Gamma");

            var midGammaDataSource = new EnumerableDataSource<Data>(midGammaValuesCollection);
            midGammaDataSource.SetXMapping(x => dateMidGamma.ConvertToDouble(x.Date));
            midGammaDataSource.SetYMapping(y => y.Value);
            plotterMidGamma.AddLineGraph(midGammaDataSource, Colors.Violet, 2, "Mid Gamma");

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

        public void OnDeviceFail(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                portTextBlock.Text = "No devices found! :( ";
            }));
        }


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
                        attentionValuesCollection.Add(new Data(tgParser.ParsedData[i]["Attention"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("Meditation"))
                {


                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        meditationTextBlock.Text = "Med Value:" + tgParser.ParsedData[i]["Meditation"];
                        meditationValuesCollection.Add(new Data(tgParser.ParsedData[i]["Meditation"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerDelta"))
                {

                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerDeltaTextBlock.Text = "Delta: " + tgParser.ParsedData[i]["EegPowerDelta"];
                        deltaValuesCollection.Add(new Data(tgParser.ParsedData[i]["EegPowerDelta"], DateTime.Now));
                    }));


                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerTheta"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerThetaTextBlock.Text = "Theta " + tgParser.ParsedData[i]["EegPowerTheta"];
                        thetaValuesCollection.Add(new Data(tgParser.ParsedData[i]["EegPowerTheta"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerAlpha1"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerAlpha1TextBlock.Text = "Alpha1: " + tgParser.ParsedData[i]["EegPowerAlpha1"];
                        lowAlphaValuesCollection.Add(new Data(tgParser.ParsedData[i]["EegPowerAlpha1"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerAlpha2"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerAlpha2TextBlock.Text = "Alpha2: " + tgParser.ParsedData[i]["EegPowerAlpha2"];
                        highAlphaValuesCollection.Add(new Data(tgParser.ParsedData[i]["EegPowerAlpha2"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerGamma1"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerGamma1TextBlock.Text = "Gamma1: " + tgParser.ParsedData[i]["EegPowerGamma1"];
                        lowGammaValuesCollection.Add(new Data(tgParser.ParsedData[i]["EegPowerGamma1"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerGamma2"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerGamma2TextBlock.Text = "Gamma2: " + tgParser.ParsedData[i]["EegPowerGamma2"];
                        midGammaValuesCollection.Add(new Data(tgParser.ParsedData[i]["EegPowerGamma2"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerBeta1"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        lowBetaValuesCollection.Add(new Data(tgParser.ParsedData[i]["EegPowerBeta1"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerBeta2"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        highBetaValuesCollection.Add(new Data(tgParser.ParsedData[i]["EegPowerBeta2"], DateTime.Now));
                    }));
                }
            }
        }
    }
}
