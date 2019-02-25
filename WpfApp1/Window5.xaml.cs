using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;

namespace MTGApro
{
    /// <summary>
    /// Логика взаимодействия для Window5.xaml
    /// </summary>
    public partial class Window5 : Window
    {
        public Window5()
        {
            InitializeComponent();  
            StreamResourceInfo sriCurs = Application.GetResourceStream(
            new Uri("pack://application:,,,/Resources/testcur.cur"));
            Cursor = new Cursor(sriCurs.Stream);
        }
    }
}
