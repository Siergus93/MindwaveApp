using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace PuzzleApp2.Windows
{
    /// <summary>
    /// Interaction logic for RankingWindow.xaml
    /// </summary>
    ///

    public partial class RankingWindow : Window
    {
        public List<Score> scoresList;

        public RankingWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //read file with ranking
            string json;
            using (StreamReader readText = new StreamReader("ranking.json"))
            {
                json = readText.ReadLine();
            }
            scoresList = JsonConvert.DeserializeObject<List<Score>>(json);
            rankingList.ItemsSource = scoresList;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            StartWindow start = new StartWindow();
            App.Current.MainWindow = start;
            this.Close();
            start.Show();
        }
    }


}

   