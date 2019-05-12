using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MTGApro
{
    /// <summary>
    /// Логика взаимодействия для Window3.xaml
    /// </summary>
    /// 

    public partial class Window3 : Window
    {

        public class Notifi
        {
            public double Date { get; set; }
            public string Txt { get; set; }

            public Notifi(int date, string txt)
            {
                Date = date;
                Txt = txt;
            }
        }

        public Window3()
        {
            string notif = MainWindow.MakeRequest(new Uri(@"https://remote.mtgarena.pro/donew.php"), new Dictionary<string, object> { { @"cmd", @"cm_getpush" }, { @"uid", MainWindow.ouruid }, { @"token", MainWindow.Usertoken } });
            Notifi[] notifparsed = Newtonsoft.Json.JsonConvert.DeserializeObject<Notifi[]>(notif);
            string output = @"";
            for (int i = 0; i <= (notifparsed.Length - 1); i++)
            {
                DateTime date = MainWindow.tmstmptodate(notifparsed[i].Date);
                string txt = notifparsed[i].Txt;
                output += @"------------------------" + Environment.NewLine + date.ToString() + @":" + Environment.NewLine + txt + Environment.NewLine + @"------------------------";
            }
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                OutputText.Text = output;
            }));

            InitializeComponent();
        }
    }
}
