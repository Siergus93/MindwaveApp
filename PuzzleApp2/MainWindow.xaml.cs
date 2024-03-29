﻿using MindwaveLib;
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
        public int attentionComingCounter;
        public int attentionValueSum;
        public int puzzlesSolved;
        public string currentWord;
        public string previousWord;
        ULoader_JSON config;
        USender uSender;

        public MainWindow()
        {
            InitializeComponent();
            foreach (string port in SerialPort.GetPortNames())
                portsComboBox.Items.Add(port);
            portsComboBox.SelectedIndex = 1;
            attentionValuesCollection = new DataCollection();
            var attentionDataSource = new EnumerableDataSource<Data>(attentionValuesCollection);
            attentionDataSource.SetXMapping(x => dateAttention.ConvertToDouble(x.Date));
            attentionDataSource.SetYMapping(y => y.Value);
            plotterAttention.AddLineGraph(attentionDataSource, Colors.Red, 2, "Attention");
            randomWordNumber = new Random();
            wordNumber = 0;
            attentionComingCounter = 0;
            attentionValueSum = 0;
            puzzlesSolved = 0;
            currentWord = "";
            config = new ULoader_JSON("config.json");
            uSender = config.GetSender("output1");
        }

        public string GetWord()
        {
            wordNumber = randomWordNumber.Next(0, MaxWordNumber);
            return ReadWord(path, wordNumber);
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
                try
                {
                    byte[] data = BitConverter.GetBytes(attentionValue);
                    uSender.SendData(data);
                    Console.WriteLine("AV: " + attentionValue + " T: " + DateTime.UtcNow.ToString());                    
                }
                catch (UException exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }));
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

        public string ReverseWord(string word)
        {
            char[] array = word.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }
    }
}
