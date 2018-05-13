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

namespace projZal
{
    /// <summary>
    /// Interaction logic for wiadomosc.xaml
    /// </summary>
    public partial class wiadomosc : Window
    {
        private string v;

        public wiadomosc()
        {
            InitializeComponent();
        }

        public wiadomosc(string v)
        {
            InitializeComponent();
            this.v = v;
            wiad.Content = v;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
