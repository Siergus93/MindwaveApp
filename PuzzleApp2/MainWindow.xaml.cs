using MindwaveLib;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using ThinkGearNET;


namespace PuzzleApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ThinkGearWrapper _thinkGearWrapper;

        public float attentionValue;

        public const int MaxWordNumber = 2709804;
        public static Random randomWordNumber;
        public string path = @"..\..\..\sjp-20151013\slowa-win-utf8.txt";
        public int wordNumber;

        public DataCollection attentionValuesCollection;

        //How many times signal came at solving a puzzle.
        public int attentionComingCounter;
        //Sum of attention signal values.
        public int attentionValueSum;
        //Collection of WordStats
        public List<WordStat> wordStatList;
        //How many puzzles have been solved.
        public int puzzlesSolved;
        //Current word
        public string currentWord;
        //Previous word
        public string previousWord;

        //Instance of object which contains configuration for sending data.
        //ULoader_JSON config;
        //Instacje of object which sends data.
        //USender uSender;


        public MainWindow()
        {
            InitializeComponent();
            foreach (string port in SerialPort.GetPortNames())
                portsComboBox.Items.Add(port);
            portsComboBox.SelectedIndex = 0;

        }

        private void _thinkGearWrapper_ThinkGearChanged(object sender, ThinkGearChangedEventArgs e)
        {
           

            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                attentionTextBlock.Text = "AT= " + e.ThinkGearState.Attention;
                poorSignalTextBlock.Text = "PS= " + e.ThinkGearState.PoorSignal;
                
            }));

            Thread.Sleep(10);
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            _thinkGearWrapper = new ThinkGearWrapper();

            _thinkGearWrapper.ThinkGearChanged += _thinkGearWrapper_ThinkGearChanged;

            if (!_thinkGearWrapper.Connect(portsComboBox.SelectedItem.ToString(), 57600, true))
            {
                MessageBox.Show("Could not connect to headset!");
            }
        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
