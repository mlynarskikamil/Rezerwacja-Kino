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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Oracle.DataAccess.Client; // ODP.NET Oracle managed provider
using Oracle.DataAccess.Types;
using System.Data;
using MySql.Data.MySqlClient;

namespace projZal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string connStr = "SERVER=lamp.ii.us.edu.pl;PORT=3306;DATABASE=ii302904;UID=ii302904;PASSWORD=Dzwonek92!;";

        MySqlConnection conn = new MySqlConnection(connStr);
        public MainWindow()
        {
            InitializeComponent();

        }

        

        private void zalogujPrzycisk_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                MySqlDataAdapter oda = new MySqlDataAdapter("SELECT * FROM WSI_CINEMA_LOGOWANIE WHERE login='" + loginBox.Text.Trim() + "' AND password='" + passBox.Password + "'", conn);
                DataTable dt = new System.Data.DataTable();
                oda.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    conn.Open();

                    string loginClient = loginBox.Text;
                    string password = passBox.Password;
                    string query = "select id_client from WSI_CINEMA_LOGOWANIE where login= '" + loginClient + "' AND password='" + password + "'";

                    MySqlCommand cmd = new MySqlCommand(query,conn);
                    string id = cmd.ExecuteScalar().ToString();

                    AfterLogin al = new AfterLogin(loginClient, password, id);
                    this.Close();
                    al.Show();

                    wiadomosc wiad = new wiadomosc("Zalogowano");
                    wiad.Show();
                    conn.Close();
                }
                else
                {
                    wiadomosc wiad = new wiadomosc("Podano błedny login lub hasło");
                    wiad.Show();
                }
            }
            catch (Exception ex)
            {
                wiadomosc wiad = new wiadomosc(ex.Message);
                wiad.Show();
            }
        }

        private void zamknij_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void zarejestrujPrzycisk_Click(object sender, RoutedEventArgs e)
        {
            //conn.ConnectionString = "SERVER=lamp.ii.us.edu.pl;PORT=3306;DATABASE=ii307664;UID=ii307664;PASSWORD=Serwer:2017;";
            conn.Open();
            try
            {
                if (wpiszHaslo.Password == wpiszPonownie.Password && !String.IsNullOrEmpty(wpiszLogin.Text) && !String.IsNullOrEmpty(wpiszHaslo.Password) && !String.IsNullOrEmpty(wpiszPonownie.Password))
                {
                    string query = "select count(*) from WSI_CINEMA_LOGOWANIE where login = '" + wpiszLogin.Text + "'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int temp = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                    if (temp == 0)
                    {
                        MySqlCommand oc = new MySqlCommand();
                        oc.Connection = conn;
                        oc.CommandText = "INSERT INTO WSI_CINEMA_LOGOWANIE (login,password) values ('" + wpiszLogin.Text + "','" + wpiszHaslo.Password + "')";
                        oc.ExecuteNonQuery();

                        wiadomosc wiad = new wiadomosc("Zarejestrowano konto");
                        wiad.Show();
                        conn.Close();
                    }

                    else
                    {
                        wiadomosc wiad = new wiadomosc("Konto o takim loginie juz istnieje");
                        wiad.Show();
                    }
                }

                else if (String.IsNullOrEmpty(wpiszLogin.Text) || String.IsNullOrEmpty(wpiszHaslo.Password) || String.IsNullOrEmpty(wpiszPonownie.Password))
                {
                    wiadomosc wiad = new wiadomosc("Któreś z pól jest puste");
                    wiad.Show();
                }
                else
                {
                    wiadomosc wiad = new wiadomosc("Podałeś dwa różne hasła");
                    wiad.Show();
                }
            }
            catch (Exception ex)
            {
                wiadomosc wiad = new wiadomosc(ex.Message);
                wiad.Show();
            }
            conn.Close();
           
        }

        private void zamknij2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void pokaz_Checked(object sender, RoutedEventArgs e)
        {
            pokazHaslo.Visibility = System.Windows.Visibility.Visible;
            passBox.Visibility = System.Windows.Visibility.Hidden;
            pokazHaslo.Text = passBox.Password;
            pokazHaslo.Focus();
        }

        private void pokaz_Unchecked(object sender, RoutedEventArgs e)
        {
            pokazHaslo.Visibility = System.Windows.Visibility.Hidden;
            passBox.Visibility = System.Windows.Visibility.Visible;
            passBox.Focus();
        }

        private void WrapPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

    }
    
}
