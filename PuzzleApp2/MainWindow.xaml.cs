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

namespace PuzzleApp2
{
    public partial class MainWindow : Window
    {
        private ThinkGearWrapper _thinkGearWrapper;

        private bool deviceIsConnected = false; 

        public float attentionValue;
        public const int MaxWordNumber = 2709804;
        public static Random randomNumber;
        public string path = @"..\..\..\sjp-20151013\slowa-win-utf8.txt";
        public int wordNumber;
        public DataCollection attentionValuesCollection;
        public int attentionComingCounter;
        public int attentionValueSum;
        public int puzzlesSolved;
        public string currentWord;
        public string previousWord;

        //matrtca 10x10, liczby od 0 do 100, losowe.., wyswietlanie jednej z liczb i szukanie jej i nastepnych...

        //- dorzucic ograniczenie czasowe do efektu strooopa i macierzy! :]

        //public int[10][10] dataTable;

        
        //StroopEffect Section!
        
        public string[] colorsStrings = { "Zielony", "Czerwony", "Niebieski", "Żółty", "Fioletowy" };
        public Color[] colorsConstants = { Colors.Green, Colors.Red, Colors.DeepSkyBlue, Colors.Yellow, Colors.DarkOrchid };
        
        public string[] puzzleChoice = { "ReversedWord", "StroopEffect", "MatrixQuest" };
                  
        public MainWindow()
        {
            
            InitializeComponent();

            _thinkGearWrapper = new ThinkGearWrapper();

            //zmienic to!

            var ports = SerialPort.GetPortNames();

            if (ports.Length > 0)
            {
                foreach (string port in SerialPort.GetPortNames())
                {
                    portsComboBox.Items.Add(port);
                }
                portsComboBox.SelectedIndex = 0;
            }
            else
                connectButton.IsEnabled = false;

            foreach (string choice in puzzleChoice)
	        {
                puzzleChoiceComboBox.Items.Add(choice);
	        }

            attentionValuesCollection = new DataCollection();
            var attentionDataSource = new EnumerableDataSource<Data>(attentionValuesCollection);
            attentionDataSource.SetXMapping(x => dateAttention.ConvertToDouble(x.Date));
            attentionDataSource.SetYMapping(y => y.Value);
            plotterAttention.AddLineGraph(attentionDataSource, Colors.Red, 2, "Attention");
            randomNumber = RandomProvider.GetThreadRandom();
            wordNumber = 0;
            attentionComingCounter = 0;
            attentionValueSum = 0;
            puzzlesSolved = 0;
            currentWord = "";       

        }

        public string GetWord()
        {
            wordNumber = randomNumber.Next(0, MaxWordNumber);
            return ReadWord(path, wordNumber);
        }

        public List<int> GetRandomNumbers(int howMany)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < howMany; )
            {
                int rand = randomNumber.Next(0, colorsStrings.Length);
                if (!result.Contains(rand))
                {
                    result.Add(rand);
                    i++;
                }                
            }

            foreach (int item in result)
            {
                Console.Write(item + " ");
                
            }
            Console.WriteLine();
            

            randomNumber = RandomProvider.GetThreadRandom();
            return result;
        }

        public List<string> GetColorsNames(List<int> inputArray)
        {
            List<string> result = new List<string>();
            for(int i = 0; i < inputArray.Count; i++)
            {
                result.Add(colorsStrings[inputArray[i]]);
            }
            return result;
        }

        public List<Color> GetColorsContants(List<int> inputArray)
        {
            List<Color> result = new List<Color>();
            for (int i = 0; i < inputArray.Count; i++)
            {
                result.Add(colorsConstants[inputArray[i]]);
            }
            return result;
        }

        public string ReadWord(string path, int line)
        {
            string text = "";
            try
            {
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8, true))
                    {
                        for (int a = 0; a < line; a++)
                        {
                            text = sr.ReadLine();
                        }
                    }
                }
            }
            catch {}
            return text;
        }

        private void _thinkGearWrapper_ThinkGearChanged(object sender, ThinkGearChangedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                attentionValue = e.ThinkGearState.Attention;
                attentionTextBlock.Text = "Att Value: " + attentionValue;
                poorSignalTextBlock.Text = "Poor Signal: " + e.ThinkGearState.PoorSignal;
                Attention.Text = attentionValue.ToString();
                attentionValuesCollection.Add(new Data(attentionValue, DateTime.Now));
                attentionValueSum += (int)attentionValue;
                attentionComingCounter += 1;
            }));
            Thread.Sleep(1000);
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            if(!deviceIsConnected)
            {
                _thinkGearWrapper.ThinkGearChanged += _thinkGearWrapper_ThinkGearChanged;
                if (!_thinkGearWrapper.Connect(portsComboBox.SelectedItem.ToString(), 57600, true))
                {
                    MessageBox.Show("Could not connect to headset!");
                }
                else
                    deviceIsConnected = true;

                connectButton.Content = "Disconnect";
            }
            else
            {
                _thinkGearWrapper.Disconnect();
                connectButton.Content = "Connect";
                deviceIsConnected = false;
            }
        }

        private void NextPuzzleButton_Click(object sender, RoutedEventArgs e)
        {
            //Reversed Word Puzzle Section
            if (puzzleChoiceComboBox.SelectedIndex == 0)
            {
                currentWord = GetWord();
                WordLabel.Content = ReverseWord(currentWord.ToUpper());
                if (puzzlesSolved > 0 && attentionComingCounter > 0)
                {
                    Console.WriteLine("w: " + previousWord + " l: " + previousWord.Length + " av: " + attentionValueSum / attentionComingCounter);
                }
                puzzlesSolved++;
                attentionComingCounter = 0;
                attentionValueSum = 0;
                previousWord = currentWord;
            }
            //Stroop Effect Puzzle Section
            else if (puzzleChoiceComboBox.SelectedIndex == 1)
            {
                //foreach (var item in GetColorsNames(GetRandomNumbers(5)))
                //{
                //    Console.WriteLine(item);
                //}
                List<string> data = GetColorsNames(GetRandomNumbers(5));
                List<Color> colorData = GetColorsContants(GetRandomNumbers(5));

                color1Label.Content = data[0];
                color1Label.Foreground = new SolidColorBrush(colorData[0]);

                color2Label.Content = data[1];
                color2Label.Foreground = new SolidColorBrush(colorData[1]);

                color3Label.Content = data[2];
                color3Label.Foreground = new SolidColorBrush(colorData[2]);

                color4Label.Content = data[3];
                color4Label.Foreground = new SolidColorBrush(colorData[3]);

                color5Label.Content = data[4];
                color5Label.Foreground = new SolidColorBrush(colorData[4]);
            }
            else if (puzzleChoiceComboBox.SelectedIndex == 2)
            {
                List<List<int>> result = MatrixGenerator.GetMatrix(36);
                matrixQuestView.ItemsSource = result;
                //matrixQuestView.Visibility = System.Windows.Visibility.Visible;
            }

        }

        public string ReverseWord(string word)
        {
            char[] array = word.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        private void puzzleChoiceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Reversed Word Puzzle Section
            if( puzzleChoiceComboBox.SelectedIndex == 0 )
            {
                WordLabel.Visibility = System.Windows.Visibility.Visible;
                colorWordsGrid.Visibility = System.Windows.Visibility.Hidden;
                matrixQuestView.Visibility = System.Windows.Visibility.Hidden;
            }
            //Stroop Effect Puzzle Section
            else if (puzzleChoiceComboBox.SelectedIndex == 1)
            {
                WordLabel.Visibility = System.Windows.Visibility.Hidden;
                colorWordsGrid.Visibility = System.Windows.Visibility.Visible;
                matrixQuestView.Visibility = System.Windows.Visibility.Hidden;

            }
            else if (puzzleChoiceComboBox.SelectedIndex == 2)
            {
                WordLabel.Visibility = System.Windows.Visibility.Hidden;
                colorWordsGrid.Visibility = System.Windows.Visibility.Hidden;
                matrixQuestView.Visibility = System.Windows.Visibility.Visible;
            }
        }

        
    }
}
