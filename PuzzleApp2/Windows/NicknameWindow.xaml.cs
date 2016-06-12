using System;
using System.Collections.Generic;
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
    /// Interaction logic for nicknameWindow.xaml
    /// </summary>
    public partial class NicknameWindow : Window
    {
        public NicknameWindow()
        {
            InitializeComponent();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            StartWindow start = new StartWindow();
            App.Current.MainWindow = start;
            this.Close();
            start.Show();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if(nicknameTextBox.Text != string.Empty)
            {
                MainWindow main = new MainWindow(nicknameTextBox.Text);
                App.Current.MainWindow = main;
                this.Close();
                main.Show();
            }
        }
    }


}
