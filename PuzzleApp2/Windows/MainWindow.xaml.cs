using MindwaveLib;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ThinkGearNET;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.IO;
using System.Timers;
using System.Linq;
using Newtonsoft.Json;

namespace PuzzleApp2.Windows
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

        public string nickname;
        public bool isWindowClosing;

        System.Windows.Threading.DispatcherTimer timer;

        public List<int> oneTrialScoresList;

        public bool isPuzzleChoiceComboBoxInitialySelected;

        public List<Score> scoresList;
                  
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        public MainWindow(string nickname)
        {
            InitializeComponent();
            Initialize();
            this.nickname = nickname;
        }

        private void Initialize()
        {
            _thinkGearWrapper = new ThinkGearWrapper();
            oneTrialScoresList = new List<int>();
            isPuzzleChoiceComboBoxInitialySelected = false;
            scoresList = new List<Score>();

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
            isWindowClosing = false;

            //demomode stuff!
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();


            
            //read file with ranking
            string json;
            using (StreamReader readText = new StreamReader("ranking.json"))
            {
                json = readText.ReadLine();
            }
            scoresList = JsonConvert.DeserializeObject<List<Score>>(json);

        }

        void timer_Tick(object sender, EventArgs e)
        {
            attentionValue = randomNumber.Next(0, 100);
            attentionTextBlock.Text = "Att Value: " + attentionValue;
            Attention.Text = attentionValue.ToString();
            attentionValuesCollection.Add(new Data(attentionValue, DateTime.Now));
            attentionValueSum += (int)attentionValue;
            attentionComingCounter += 1;
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
                timer.Stop();
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
                timer.Start();
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
                    oneTrialScoresList.Add((int)(attentionValueSum / attentionComingCounter));
                }
                puzzlesSolved++;
                attentionComingCounter = 0;
                attentionValueSum = 0;
                previousWord = currentWord;
            }
            //Stroop Effect Puzzle Section
            else if (puzzleChoiceComboBox.SelectedIndex == 1)
            {
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


                if (puzzlesSolved > 0 && attentionComingCounter > 0)
                {
                    Console.WriteLine(" av: " + attentionValueSum / attentionComingCounter);
                    oneTrialScoresList.Add((int)(attentionValueSum / attentionComingCounter));
                }
                puzzlesSolved++;
                attentionComingCounter = 0;
                attentionValueSum = 0;

            }
            else if (puzzleChoiceComboBox.SelectedIndex == 2)
            {
                int matrixNumberOfElementsBadasss = 49;
                List<List<string>> result = MatrixGenerator.GetMatrix(matrixNumberOfElementsBadasss);
                matrixQuestView.ItemsSource = result;
                matrixQuestTextBlock.Text = MatrixGenerator.GetFinding(matrixNumberOfElementsBadasss);

                if (puzzlesSolved > 0 && attentionComingCounter > 0)
                {
                    Console.WriteLine(" av: " + attentionValueSum / attentionComingCounter);
                    oneTrialScoresList.Add((int)(attentionValueSum / attentionComingCounter));
                }
                puzzlesSolved++;
                attentionComingCounter = 0;
                attentionValueSum = 0;

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
                matrixQuestTextBlock.Visibility = System.Windows.Visibility.Hidden;
            }
            //Stroop Effect Puzzle Section
            else if (puzzleChoiceComboBox.SelectedIndex == 1)
            {
                WordLabel.Visibility = System.Windows.Visibility.Hidden;
                colorWordsGrid.Visibility = System.Windows.Visibility.Visible;
                matrixQuestView.Visibility = System.Windows.Visibility.Hidden;
                matrixQuestTextBlock.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (puzzleChoiceComboBox.SelectedIndex == 2)
            {
                WordLabel.Visibility = System.Windows.Visibility.Hidden;
                colorWordsGrid.Visibility = System.Windows.Visibility.Hidden;
                matrixQuestView.Visibility = System.Windows.Visibility.Visible;
                matrixQuestTextBlock.Visibility = System.Windows.Visibility.Visible;
            }

            if (isPuzzleChoiceComboBoxInitialySelected)
            {
                Score score = new Score(nickname, puzzleChoice[puzzleChoiceComboBox.SelectedIndex] , GetAverageValueFromList(oneTrialScoresList));
                Console.WriteLine("p: " + nickname + " t: " + puzzleChoice[puzzleChoiceComboBox.SelectedIndex] + " a: " + GetAverageValueFromList(oneTrialScoresList));
                oneTrialScoresList.Clear();

                scoresList.Add(score);
                scoresList = scoresList.OrderByDescending(s => s.scoreValue).Take(10).ToList();
                string json = JsonConvert.SerializeObject(scoresList);

                //...zapis do pliku!

                using(StreamWriter writeText = new StreamWriter("ranking.json"))
                {
                    writeText.Write(json);
                }
                int i = 0;


                
            }
            else isPuzzleChoiceComboBoxInitialySelected = true;


        }

        public int GetAverageValueFromList(List<int> list)
        {
            if (list.Count <= 0)
                return 0;
            else
            {
                int result = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    result += list[i];
                }
                return (int)(result / list.Count);
            }
        }

        private void backToStartButton_Click(object sender, RoutedEventArgs e)
        {
            AreYouSureToQuitWindow quit = new AreYouSureToQuitWindow(this, true);
            App.Current.MainWindow = quit;
            this.Hide();
            quit.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!isWindowClosing)
            {
                e.Cancel = true;

                isWindowClosing = true;

                AreYouSureToQuitWindow quit = new AreYouSureToQuitWindow(this, false);
                App.Current.MainWindow = quit;
                this.Hide();
                quit.Show(); 
            }
        }


        

        
    }
}
