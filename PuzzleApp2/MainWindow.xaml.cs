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

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.IO;

using libUCLA;


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
        ULoader_JSON config;
        //Instacje of object which sends data.
        USender uSender;




        public MainWindow()
        {
            InitializeComponent();
            foreach (string port in SerialPort.GetPortNames())
                portsComboBox.Items.Add(port);
            portsComboBox.SelectedIndex = 0;

            attentionValuesCollection = new DataCollection();

            var attentionDataSource = new EnumerableDataSource<DataValue>(attentionValuesCollection);
            attentionDataSource.SetXMapping(x => dateAttention.ConvertToDouble(x.Date));
            attentionDataSource.SetYMapping(y => y.Value);
            plotterAttention.AddLineGraph(attentionDataSource, Colors.Red, 2, "Attetion");

            //Stworzenie obiektu typu Random do wyboru losowego słowa ze słownika.
            randomWordNumber = new Random();

            wordNumber = 0;

            attentionComingCounter = 0;
            attentionValueSum = 0;
            puzzlesSolved = 0;
            wordStatList = new List<WordStat>();
            currentWord = "";


            config = new ULoader_JSON("config.json");
            uSender = config.GetSender("output1");

        }

        public string GetAWord()
        {
            wordNumber = randomWordNumber.Next(0, MaxWordNumber);

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
            catch
            {

            }
            return result;
        }

        private void _thinkGearWrapper_ThinkGearChanged(object sender, ThinkGearChangedEventArgs e)
        {
           

            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                attentionTextBlock.Text = "Att Value: " + e.ThinkGearState.Attention;
                poorSignalTextBlock.Text = "Poor Signal: " + e.ThinkGearState.PoorSignal;
                Attention.Text = e.ThinkGearState.Attention.ToString();
                attentionValuesCollection.Add(new DataValue(e.ThinkGearState.Attention, DateTime.Now));

                attentionValueSum += (int)e.ThinkGearState.Attention;
                attentionComingCounter += 1;

                try
                {
                    //ULoader_JSON config = new ULoader_JSON("config.json");
                    //USender uSender = config.GetSender("output1");
                    //byte[] data = new byte[1];
                    //data[0] = Byte.Parse(e.ThinkGearState.Attention.ToString());
                    uSender.SendData(BitConverter.GetBytes(e.ThinkGearState.Attention));
                    //uSender.SendData(BitConverter.GetBytes(attentionComingCounter));
                    
                }
                catch (UException exc)
                {
                    Console.WriteLine(exc.Message);
                }

            }));

            //attentionTextBlock.Text = "Att Value:" + tgParser.ParsedData[i]["Attention"];
            //Attention.Text = tgParser.ParsedData[i]["Attention"].ToString();
            //attentionValuesCollection.Add(new DataValue(tgParser.ParsedData[i]["Attention"], DateTime.Now));

            //Puzzle solving statistics.
            //attentionValueSum += (int)tgParser.ParsedData[i]["Attention"];
            //attentionComingCounter += 1;

            //Sending data to another app.
            
            Thread.Sleep(1000);
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
            currentWord = GetAWord();
            WordLabel.Content = Reverse(currentWord.ToUpper());

            if (puzzlesSolved > 0 && attentionComingCounter > 0)
            {
                //wordStatList.Add(new WordStat { WordNumber = wordNumber, AverageAttentionValue = attentionValueSum / attentionComingCounter });
                //w - word, l - length, av - average value of Attention
                Console.WriteLine("w: " + previousWord + " l: " + previousWord.Length + " av: " + attentionValueSum / attentionComingCounter);
                puzzlesSolved++;
            }
            else puzzlesSolved++;


            attentionComingCounter = 0;
            attentionValueSum = 0;
            previousWord = currentWord;
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
