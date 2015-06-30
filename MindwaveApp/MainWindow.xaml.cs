using System;
using System.Windows;
using System.Windows.Media;

using NeuroSky.ThinkGear;
using System.Windows.Threading;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.ComponentModel;


namespace MindwaveApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window//, INotifyPropertyChanged
    {
        public static Connector connector;
        public static byte poorSig;


        // Kolekcja wartości Attention pobranych z urzadzenia.
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
        

        //DispatcherTimer updateCollection;

        //#region OnPropertyChangeUpdate Updating values

        //private int _maxAttetion;
        //private int _minAttetion;

        //public int MaxAttetion
        //{
        //    get;
        //    set;
        //    //get { return _maxAttetion; }
        //    //set { _maxAttetion = value; this.OnPropertyChanged("MaxAttention"); }
        //}

        //public int MinAttetion
        //{
        //    get;
        //    set;
        //    //get { return _minAttetion; }
        //    //set { _minAttetion = value; this.OnPropertyChanged("MinAttention"); }
        //}

        //#endregion

        //private int i = 0;


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            // Stworzenie nowej kolekcji.
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

            var attentionDataSource = new EnumerableDataSource<DataValue>(attentionValuesCollection);
            attentionDataSource.SetXMapping(x => dateAttention.ConvertToDouble(x.Date));
            attentionDataSource.SetYMapping(y => y.Value);
            plotterAttention.AddLineGraph(attentionDataSource, Colors.Red, 2, "Attetion");

            var meditationDataSource = new EnumerableDataSource<DataValue>(meditationValuesCollection);
            meditationDataSource.SetXMapping(x => dateMeditation.ConvertToDouble(x.Date));
            meditationDataSource.SetYMapping(y => y.Value);
            plotterMeditation.AddLineGraph(meditationDataSource, Colors.Green, 2, "Meditation");



            var deltaDataSource = new EnumerableDataSource<DataValue>(deltaValuesCollection);
            deltaDataSource.SetXMapping(x => dateDelta.ConvertToDouble(x.Date));
            deltaDataSource.SetYMapping(y => y.Value);
            plotterDelta.AddLineGraph(deltaDataSource, Colors.Blue, 2, "Delta");

            var thetaDataSource = new EnumerableDataSource<DataValue>(thetaValuesCollection);
            thetaDataSource.SetXMapping(x => dateTheta.ConvertToDouble(x.Date));
            thetaDataSource.SetYMapping(y => y.Value);
            plotterTheta.AddLineGraph(thetaDataSource, Colors.Cyan, 2, "Theta");



            var lowAlphaDataSource = new EnumerableDataSource<DataValue>(lowAlphaValuesCollection);
            lowAlphaDataSource.SetXMapping(x => dateLowAlpha.ConvertToDouble(x.Date));
            lowAlphaDataSource.SetYMapping(y => y.Value);
            plotterLowAlpha.AddLineGraph(lowAlphaDataSource, Colors.Crimson, 2, "Low Alpha");

            var highAlphaDataSource = new EnumerableDataSource<DataValue>(highAlphaValuesCollection);
            highAlphaDataSource.SetXMapping(x => dateHighAlpha.ConvertToDouble(x.Date));
            highAlphaDataSource.SetYMapping(y => y.Value);
            plotterHighAlpha.AddLineGraph(highAlphaDataSource, Colors.Indigo, 2, "High Alpha");



            var lowBetaDataSource = new EnumerableDataSource<DataValue>(lowBetaValuesCollection);
            lowBetaDataSource.SetXMapping(x => dateTheta.ConvertToDouble(x.Date));
            lowBetaDataSource.SetYMapping(y => y.Value);
            plotterLowBeta.AddLineGraph(lowBetaDataSource, Colors.Orchid, 2, "Low Beta");

            var highBetaDataSource = new EnumerableDataSource<DataValue>(highBetaValuesCollection);
            highBetaDataSource.SetXMapping(x => dateTheta.ConvertToDouble(x.Date));
            highBetaDataSource.SetYMapping(y => y.Value);
            plotterHighBeta.AddLineGraph(highBetaDataSource, Colors.Blue, 2, "High Beta");



            var lowGammaDataSource = new EnumerableDataSource<DataValue>(lowGammaValuesCollection);
            lowGammaDataSource.SetXMapping(x => dateLowGamma.ConvertToDouble(x.Date));
            lowGammaDataSource.SetYMapping(y => y.Value);
            plotterLowGamma.AddLineGraph(lowGammaDataSource, Colors.Purple, 2, "Low Gamma");

            var midGammaDataSource = new EnumerableDataSource<DataValue>(midGammaValuesCollection);
            midGammaDataSource.SetXMapping(x => dateMidGamma.ConvertToDouble(x.Date));
            midGammaDataSource.SetYMapping(y => y.Value);
            plotterMidGamma.AddLineGraph(midGammaDataSource, Colors.Violet, 2, "Mid Gamma");


            // Stworzenie Connectora, czyli obiektu łączącego się z urządzeniem.
            connector = new Connector();
            connector.DeviceConnected += new EventHandler(OnDeviceConnected);
            connector.DeviceConnectFail += new EventHandler(OnDeviceFail);
            connector.DeviceValidating += new EventHandler(OnDeviceValidating);

            connector.ConnectScan("COM3");

            
            
            // Włączenie detekcji mrugania.
            //connector.setBlinkDetectionEnabled(true);


            

        }



        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            connector.Close();
            Environment.Exit(0);
        }

        //void updateCollection_Tick(object sender, EventArgs e)
        //{
        //    i++;
        //    attentionValuesCollection.Add(new AttentionValue( Math.Sin(i*0.1), DateTime.Now ));
        //}

        //private async void Wait(int value)
        //{
        //    await Task.Delay(TimeSpan.FromMilliseconds(value));
        //}

        // Called when a device is connected 



        public void OnDeviceConnected(object sender, EventArgs e)
        {
           

            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            //this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => textBlock1.Text += "Device found on: " + de.Device.PortName);

            //textBlock1.Text += "Device found on: " + de.Device.PortName;

            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                
                portTextBlock.Text = "Device found on: " + de.Device.PortName;
            }));
            de.Device.DataReceived += new EventHandler(OnDataReceived);

            /// ????
            
            //updateCollection.Tick += new EventHandler(updateCollection_Tick);
            //updateCollection.Start();

        }




        // Called when scanning fails

        public void OnDeviceFail(object sender, EventArgs e)
        {

            //this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => textBlock1.Text += "No devices found! :( ");
            //textBlock1.Text += "No devices found! :( ";
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                portTextBlock.Text = "No devices found! :( ";
            }));
        }



        // Called when each port is being validated

        public void OnDeviceValidating(object sender, EventArgs e)
        {
            //this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => textBlock1.Text += "Validating: ");

            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                validatingTextBlock.Text += "Vali: ";
            }));

        }

        // Called when data is received from a device

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

                //if (tgParser.ParsedData[i].ContainsKey("Raw"))
                //{

                //    //Console.WriteLine("Raw Value:" + tgParser.ParsedData[i]["Raw"]);
                //    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                //    {
                //        rawDataTextBlock.Text = "Raw Data: " + tgParser.ParsedData[i]["Raw"];
                //    }));

                //}

                if (tgParser.ParsedData[i].ContainsKey("PoorSignal"))
                {

                    //The following line prints the Time associated with the parsed data
                    //Console.WriteLine("Time:" + tgParser.ParsedData[i]["Time"]);

                    //A Poor Signal value of 0 indicates that your headset is fitting properly
                    //Console.WriteLine("Poor Signal:" + tgParser.ParsedData[i]["PoorSignal"]);
                    //textBlock1.Text = "Poor Signal:" + tgParser.ParsedData[i]["PoorSignal"];

                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        poorSignalTextBlock.Text = "Poor Signal:" + tgParser.ParsedData[i]["PoorSignal"];
                        
                    }));

                    //poorSig = (byte)tgParser.ParsedData[i]["PoorSignal"];
                }


                if (tgParser.ParsedData[i].ContainsKey("Attention"))
                {

                    //Console.WriteLine("Att Value:" + tgParser.ParsedData[i]["Attention"]);
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        attentionTextBlock.Text = "Att Value:" + tgParser.ParsedData[i]["Attention"];
                        attentionValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["Attention"], DateTime.Now));
                    }));
           
                }


                if (tgParser.ParsedData[i].ContainsKey("Meditation"))
                {

                    //Console.WriteLine("Med Value:" + tgParser.ParsedData[i]["Meditation"]);

                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        meditationTextBlock.Text = "Med Value:" + tgParser.ParsedData[i]["Meditation"];
                        meditationValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["Meditation"], DateTime.Now));
                    }));
                }


                if (tgParser.ParsedData[i].ContainsKey("EegPowerDelta"))
                {
                    //Console.WriteLine("Delta: " + tgParser.ParsedData[i]["EegPowerDelta"]);

                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerDeltaTextBlock.Text = "Delta: " + tgParser.ParsedData[i]["EegPowerDelta"];
                        deltaValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["EegPowerDelta"], DateTime.Now));
                        //MakeRelative(deltaValuesCollection);
                    }));


                }

                //if (tgParser.ParsedData[i].ContainsKey("BlinkStrength"))
                //{

                //    //Console.WriteLine("Eyeblink " + tgParser.ParsedData[i]["BlinkStrength"]);

                //    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                //    {
                //        blinkStrengthTextBlock.Text = "Eyeblink " + tgParser.ParsedData[i]["BlinkStrength"];
                        
                //    }));

                //}

                if (tgParser.ParsedData[i].ContainsKey("EegPowerTheta"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerThetaTextBlock.Text = "Theta " + tgParser.ParsedData[i]["EegPowerTheta"];
                        thetaValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["EegPowerTheta"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerAlpha1"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerAlpha1TextBlock.Text = "Alpha1: " + tgParser.ParsedData[i]["EegPowerAlpha1"];
                        lowAlphaValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["EegPowerAlpha1"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerAlpha2"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerAlpha2TextBlock.Text = "Alpha2: " + tgParser.ParsedData[i]["EegPowerAlpha2"];
                        highAlphaValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["EegPowerAlpha2"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerGamma1"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerGamma1TextBlock.Text = "Gamma1: " + tgParser.ParsedData[i]["EegPowerGamma1"];
                        lowGammaValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["EegPowerGamma1"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerGamma2"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        eegPowerGamma2TextBlock.Text = "Gamma2: " + tgParser.ParsedData[i]["EegPowerGamma2"];
                        midGammaValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["EegPowerGamma2"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerBeta1"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        //eegPowerGamma2TextBlock.Text = "Gamma2: " + tgParser.ParsedData[i]["EegPowerGamma2"];
                        lowBetaValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["EegPowerBeta1"], DateTime.Now));
                    }));
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerBeta2"))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        //eegPowerGamma2TextBlock.Text = "Gamma2: " + tgParser.ParsedData[i]["EegPowerGamma2"];
                        highBetaValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["EegPowerBeta2"], DateTime.Now));
                    }));
                }


                

            }

        }

        //#region INotifyPropertyChanged members

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //        this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        //}

        //#endregion

        public void MakeRelative(DataCollection collection)
        {
            DataCollection temp = new DataCollection();

            //foreach (var item in collection)
            //{
            //    temp.Add(new DataValue(collection[item].Value / collection.MaxValue, collection[item].Date));
            //}

            for (int i = 0; i < collection.Count; i++)
            {
                temp.Add(new DataValue( (collection[i].Value / collection.MaxValue) * 100, collection[i].Date));
            }

            collection = temp;
        }
    }
}
