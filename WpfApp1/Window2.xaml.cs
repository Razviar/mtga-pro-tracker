using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MTGApro
{
    /// <summary>
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string command = (sender as Button).Name.ToString();
                if (command == @"decks")
                {
                    Uri link = new Uri(@"https://mtgarena.pro/decks/");
                    Process.Start(new ProcessStartInfo(link.AbsoluteUri));
                }
                else if (command == @"mydecks")
                {
                    Uri link = new Uri(@"https://mtgarena.pro/decks/?my");
                    Process.Start(new ProcessStartInfo(link.AbsoluteUri));
                }
                else if (command == @"mycol")
                {
                    Uri link = new Uri(@"https://mtgarena.pro/collection/");
                    Process.Start(new ProcessStartInfo(link.AbsoluteUri));
                }
                else if (command == @"myprogr")
                {
                    Uri link = new Uri(@"https://mtgarena.pro/progress/");
                    Process.Start(new ProcessStartInfo(link.AbsoluteUri));
                }
                else if (command == @"deckbuilder")
                {
                    Uri link = new Uri(@"https://mtgarena.pro/deckbuilder/");
                    Process.Start(new ProcessStartInfo(link.AbsoluteUri));
                }
            }
            catch(Exception)
            {

            }
        }
    }
}
