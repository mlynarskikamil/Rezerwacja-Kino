using MySql.Data.MySqlClient;
using Oracle.DataAccess.Client;//+
using System;
using System.Collections.Generic;
using System.Data;//+
using System.IO;//+
using System.Linq;
using System.Net;//+
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
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace projZal
{
    /// <summary>
    /// Interaction logic for AfterLogin.xaml
    /// </summary>
    public partial class AfterLogin : Window
    {
        private string loginClient;
        private string password;
        private string id;

        public AfterLogin(string loginClient, string password, string id)
        {
            InitializeComponent();
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 0.6;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 0.6;

            this.loginClient = loginClient;
            this.password = password;
            this.id = id;
        }

        private void uiCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (myRes.IsSelected)
            {
                const String buttonContent = "Usuń...";
                String iconsQuery = "SELECT DISTINCT f.ID_FILM, f.ID_TMDB, f.URL_TRAILER " +
                                    "FROM WSI_CINEMA_REZERWACJA r " +
                                    "JOIN WSI_CINEMA_SEANS s ON(r.id_seans = s.id_seans) " +
                                    "JOIN WSI_CINEMA_FILM f ON(s.id_film = f.id_film) " +
                                    "WHERE r.ID_CLIENT = " + id;
                drawHudIcons(myResWP, actionDelete, buttonContent, iconsQuery);
                /*
                DataTable table = Manage.hudCreate(myResWP, actionDelete, buttonContent, "SELECT DISTINCT f.ID_FILM, f.ID_TMDB, f.URL_TRAILER " +
                                                                   "FROM WSI_CINEMA_REZERWACJA r " +
                                                                   "JOIN WSI_CINEMA_SEANS s ON(r.id_seans = s.id_seans) " +
                                                                   "JOIN WSI_CINEMA_FILM f ON(s.id_film = f.id_film) " +
                                                                   "WHERE r.ID_CLIENT = " + id);
                                                                   */
            }
            else if (res.IsSelected)
            {
                const String buttonContent = "Rezerwuj...";
                String iconsQuery = "SELECT DISTINCT f.ID_FILM, f.ID_TMDB, f.URL_TRAILER " +
                                    "FROM WSI_CINEMA_FILM f " +
                                    "WHERE f.ID_FILM IN(SELECT s.ID_FILM FROM WSI_CINEMA_SEANS s)";
                drawHudIcons(resWP, actionReservation, buttonContent, iconsQuery);

                /*
                DataTable table = Manage.hudCreate(resWP, actionReservation, "Rezerwuj...", "SELECT DISTINCT f.ID_FILM, f.ID_TMDB, f.URL_TRAILER " +
                                                                          "FROM WSI_CINEMA_FILM f " +
                                                                          "WHERE f.ID_FILM IN(SELECT s.ID_FILM FROM WSI_CINEMA_SEANS s)");
                                                                          */
                /*
                Manage.hudCreate(resWP, actionReservation, "Rezerwuj...", "SELECT DISTINCT f.ID_FILM, f.ID_TMDB, f.URL_TRAILER " +
                                                                          "FROM WSI_CINEMA_FILM f " +
                                                                          "WHERE f.ID_FILM IN(SELECT s.ID_FILM FROM WSI_CINEMA_SEANS s)");*/
            }
        }

        private void drawHudIcons(WrapPanel panel, Action<object, RoutedEventArgs> action, string aButtCon, string query)
        {
            DataTable queryResult = new DataTable();
            Manage.dbConTab(query).Fill(queryResult);
            panel.Children.Clear();

            if (queryResult.Rows.Count == 0)
            {
                Label notFound = new Label();
                notFound.Content = "Brak elementow do wyswietlenia";
                notFound.FontSize = 32;
                panel.Children.Add(notFound);
            }
            else
            {
                drawIconsThread(panel, action, aButtCon, queryResult, 0, 4);
                drawIconsThread(panel, action, aButtCon, queryResult, 1, 4);
                drawIconsThread(panel, action, aButtCon, queryResult, 2, 4);
                drawIconsThread(panel, action, aButtCon, queryResult, 3, 4);
            }
        }
        private void drawIconsThread(WrapPanel panel, Action<object, RoutedEventArgs> action, string aButtCon, DataTable icons, int startpoint, int incrementBy)
        {
            Task.Factory.StartNew(() =>
            {
                for (int i = startpoint; i < icons.Rows.Count; i += incrementBy)
                {
                    int j = i;
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        DataRow row = icons.Rows[j];
                        ActionButton aButt = new ActionButton(row[0].ToString(), aButtCon, action);
                        MoreButton mButt = new MoreButton(row[1].ToString(), row[2].ToString());

                        Movie movie = new TMDbClient("7e8f60e325cd06e164799af1e317d7a7").GetMovieAsync(Convert.ToInt32(row[1]), "pl", MovieMethods.Credits).Result;
                        MovieIcon icon = new MovieIcon(mButt, aButt, "https://image.tmdb.org/t/p/w500/" + movie.PosterPath);

                        panel.Children.Add(icon);
                    }));
                }
            });
        }

        private void zatwierdzButton_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = "SERVER=lamp.ii.us.edu.pl;PORT=3306;DATABASE=ii302904;UID=ii302904;PASSWORD=Dzwonek92!;";
            try
            {
                if (stareHasloBox.Password == password && !String.IsNullOrEmpty(noweHasloBox.Password) && !String.IsNullOrEmpty(nowePotwierdzBox.Password))
                {
                    conn.Open();
                    MySqlCommand oc = new MySqlCommand();
                    oc.Connection = conn;
                    oc.CommandText = "UPDATE logowanie SET password='" + noweHasloBox.Password + "' WHERE id_client='" + id + "'";
                    oc.ExecuteNonQuery();

                    wiadomosc wiad = new wiadomosc("Zmieniono hasło");
                    wiad.Show();
                    conn.Close();
                }
                else if (String.IsNullOrEmpty(noweHasloBox.Password) || String.IsNullOrEmpty(stareHasloBox.Password) || String.IsNullOrEmpty(nowePotwierdzBox.Password))
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
        }

        private void actionReservation(object sender, RoutedEventArgs e)
        {
            new Reservation(id, ((ActionButton)sender).getMovieId()).ShowDialog();
        }

        private void actionDelete(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("W dlc");
        }

        private void WrapPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void logout(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            this.Close();
            mw.Show();
        }
    }

    public class MovieIcon : Grid
    {
        private Image icon;
        private MoreButton mButt;
        private ActionButton aButt;

        public MovieIcon(MoreButton mButt, ActionButton aButt, string url, double left = 20, double top = 10, int Width = 200, int Height = 285)
        {
            this.mButt = mButt;
            this.aButt = aButt;
            Margin = new Thickness(left, top, 0, 0);
            icon = new Image();
            icon.Width = Width;
            icon.Height = Height;
            icon.Source = new BitmapImage(new Uri(url));
            icon.Stretch = Stretch.Fill;
            icon.Opacity = 0.5;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            MouseEnter += iconEnterAnim;
            MouseLeave += iconLeaveAnim;
            this.Children.Add(icon);
            this.Children.Add(mButt);
            this.Children.Add(aButt);
        }

        public MoreButton getMoreButton()
        {
            return mButt;
        }

        public ActionButton getActionButton()
        {
            return aButt;
        }

        private void iconEnterAnim(object sender, MouseEventArgs e)
        {
            ((MovieIcon)sender).Opacity = 1;
            ((MovieIcon)sender).getMoreButton().Visibility = Visibility.Visible;
            ((MovieIcon)sender).getActionButton().Visibility = Visibility.Visible;
        }

        private void iconLeaveAnim(object sender, MouseEventArgs e)
        {
            ((MovieIcon)sender).Opacity = 0.5;
            ((MovieIcon)sender).getMoreButton().Visibility = Visibility.Hidden;
            ((MovieIcon)sender).getActionButton().Visibility = Visibility.Hidden;
        }
    }

    public class ActionButton : Button
    {
        private string movieId;

        public ActionButton(string movieId, string Content, Action<object, RoutedEventArgs> action, HorizontalAlignment hAlign = HorizontalAlignment.Right, double left = 5, double right = 5, double bottom = 5, int Width = 75)
        {
            this.movieId = movieId;
            this.Content = Content;
            HorizontalAlignment = hAlign;
            Margin = new Thickness(left, 0, right, bottom);
            this.Width = Width;
            VerticalAlignment = VerticalAlignment.Bottom;
            Visibility = Visibility.Hidden;
            Click += new RoutedEventHandler(action);
        }

        public string getMovieId()
        {
            return movieId;
        }
    }

    public class MoreButton : ActionButton
    {
        private string url;

        public MoreButton(string movieId, string url) : base(movieId, "Więcej...", openMore, HorizontalAlignment.Left)
        {
            this.url = url;
        }

        public string getUrl()
        {
            return url;
        }

        private static void openMore(object sender, RoutedEventArgs e)
        {
            new More(((MoreButton)sender).getMovieId(), ((MoreButton)sender).getUrl()).ShowDialog();
        }
    }
}