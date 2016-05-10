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
    /// Interaction logic for AreSureToQuitWindow.xaml
    /// </summary>
    public partial class AreYouSureToQuitWindow : Window
    {
        private MainWindow mainWindow;
        private bool isBack;

        public AreYouSureToQuitWindow()
        {
            InitializeComponent();
        }

        public AreYouSureToQuitWindow(MainWindow mainWindow, bool isBack)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.isBack = isBack;
        }

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isBack)
            {
                mainWindow.Close();
                this.Close();
            }
            else
            {
                mainWindow.isWindowClosing = true;
                StartWindow start = new StartWindow();
                App.Current.MainWindow = start;
                start.Show(); 

                mainWindow.Close();
                this.Close();
            }

            //... saving procedure....
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.isWindowClosing = false;
            App.Current.MainWindow = mainWindow;
            mainWindow.Show();
            this.Close();
        }
    }
}
