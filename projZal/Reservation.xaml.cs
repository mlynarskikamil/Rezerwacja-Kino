using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Reservation.xaml
    /// </summary>
    public partial class Reservation : Window
    {
        private string idUser;
        private string idFilm;
        private DataTable reservedSeats = new DataTable();

        public Reservation(string idUser, string idFilm)
        {
            InitializeComponent();
            this.idUser = idUser;
            this.idFilm = idFilm;
            reservedSeats.Columns.Add("id", typeof(int));

            DataTable queryResult = new DataTable();

            Manage.dbConTab("SELECT DISTINCT DATE_FORMAT(r.TERMIN,'%d.%m.%y') " +
                            "FROM WSI_CINEMA_SEANS s " +
                            "JOIN WSI_CINEMA_REPERTUAR r ON(s.ID_REPERTUAR = r.ID_REPERTUAR) " +
                            "WHERE s.ID_FILM = " + idFilm).Fill(queryResult);

            termin.ItemsSource = queryResult.DefaultView;
            termin.DisplayMemberPath = queryResult.Columns[0].ToString();
            termin.SelectedValuePath = queryResult.Columns[0].ToString();
            if (queryResult.Rows.Count == 1) termin.SelectedIndex = 0;
        }

        private void termin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataTable queryResult = new DataTable();
            Manage.dbConTab("SELECT DISTINCT s.CZAS_ROZPOCZECIA " +
                             "FROM WSI_CINEMA_SEANS s " +
                             "JOIN WSI_CINEMA_REPERTUAR r ON(s.ID_REPERTUAR = r.ID_REPERTUAR) " +
                             "WHERE s.ID_FILM = " + idFilm + " AND DATE_FORMAT(r.TERMIN,'%d.%m.%y') = " + "'" + termin.SelectedValue + "'").Fill(queryResult);

            czas.ItemsSource = queryResult.DefaultView;
            czas.DisplayMemberPath = queryResult.Columns[0].ToString();
            czas.SelectedValuePath = queryResult.Columns[0].ToString();
            reservedSeats.Clear();
            resNow.Visibility = Visibility.Hidden;
            if (queryResult.Rows.Count == 1) czas.SelectedIndex = 0;
        }

        private void czas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataTable queryResult = new DataTable();
            Manage.dbConTab("SELECT r.nr_sala " +
                             "FROM WSI_CINEMA_SEANS s " +
                             "JOIN WSI_CINEMA_REPERTUAR r ON(s.ID_REPERTUAR = r.ID_REPERTUAR) " +
                             "WHERE s.ID_FILM = " + idFilm + " AND DATE_FORMAT(r.TERMIN,'%d.%m.%y') = '" + termin.SelectedValue + "'" +
                             " AND s.CZAS_ROZPOCZECIA = " + "'" + czas.SelectedValue + "'").Fill(queryResult);

            sala.ItemsSource = queryResult.DefaultView;
            sala.DisplayMemberPath = queryResult.Columns[0].ToString();
            sala.SelectedValuePath = queryResult.Columns[0].ToString();
            reservedSeats.Clear();
            resNow.Visibility = Visibility.Hidden;
            if (queryResult.Rows.Count == 1) sala.SelectedIndex = 0;
        }

        private void sala_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            seatScheme.Children.Clear();
            reservedSeats.Clear();
            resNow.Visibility = Visibility.Hidden;
            if (sala.SelectedIndex != -1)
            {
                DataTable freeSeats = new DataTable();
                DataTable takenSeats = new DataTable();

                Manage.dbConTab(
                                 "SELECT ID_SIEDZENIE, NR_SIEDZENIE " +
                                 "FROM WSI_CINEMA_SIEDZENIE " +
                                 "WHERE NR_SALA = " + sala.SelectedValue + " " +
                                 "AND ID_SIEDZENIE NOT IN(" +
                                 "SELECT rez.ID_SIEDZENIE " +
                                 "FROM WSI_CINEMA_REZERWACJA rez " +
                                 "JOIN WSI_CINEMA_SEANS se ON(rez.ID_SEANS = se.ID_SEANS) " +
                                 "JOIN WSI_CINEMA_REPERTUAR rep ON(se.ID_REPERTUAR = rep.ID_REPERTUAR) " +
                                 "JOIN WSI_CINEMA_SIEDZENIE si ON(si.ID_SIEDZENIE = rez.ID_SIEDZENIE) " +
                                 "WHERE se.ID_FILM = " + idFilm + " AND DATE_FORMAT(rep.TERMIN,'%d.%m.%y') = '" + termin.SelectedValue +
                                 "' AND se.CZAS_ROZPOCZECIA = '" + czas.SelectedValue + "' AND rep.NR_SALA = " + sala.SelectedValue + ")"
                                 ).Fill(freeSeats);


                Manage.dbConTab("SELECT rez.ID_SIEDZENIE, si.NR_SIEDZENIE " +
                                 "FROM WSI_CINEMA_REZERWACJA rez " +
                                 "JOIN WSI_CINEMA_SEANS se ON(rez.ID_SEANS = se.ID_SEANS) " +
                                 "JOIN WSI_CINEMA_REPERTUAR rep ON(se.ID_REPERTUAR = rep.ID_REPERTUAR) " +
                                 "JOIN WSI_CINEMA_SIEDZENIE si ON(si.ID_SIEDZENIE = rez.ID_SIEDZENIE) " +
                                 "WHERE se.ID_FILM = " + idFilm + " AND DATE_FORMAT(rep.TERMIN,'%d.%m.%y') = '" + termin.SelectedValue +
                                 "' AND se.CZAS_ROZPOCZECIA = '" + czas.SelectedValue + "' AND rep.NR_SALA = " + sala.SelectedValue).Fill(takenSeats);

                int freeCreated = 0;
                int takenCreated = 0;
                WrapPanel line = null;
                const int LINE_LENGTH = 10;
                while (freeCreated < freeSeats.Rows.Count && takenCreated < takenSeats.Rows.Count)
                {
                    if ((freeCreated + takenCreated) % LINE_LENGTH == 0)
                    {
                        seatScheme.Children.Add(line = new WrapPanel());
                        line.HorizontalAlignment = HorizontalAlignment.Center;
                    }

                    int FREE_SEATS_ID = Convert.ToInt32(freeSeats.Rows[freeCreated][1]);
                    int TAKEN_SEATS_ID = Convert.ToInt32(takenSeats.Rows[takenCreated][1]);

                    if ( FREE_SEATS_ID < TAKEN_SEATS_ID)
                    {
                        int id = Convert.ToInt32(freeSeats.Rows[freeCreated][0]);
                        int seatNumber = Convert.ToInt32(freeSeats.Rows[freeCreated][1]);
                        line.Children.Add(new freeSeat( id, seatNumber, addReserv));
                        freeCreated++;
                    }
                    else
                    {
                        line.Children.Add(new takenSeat());
                        takenCreated++;
                    }
                }

                while (freeCreated != freeSeats.Rows.Count)
                {
                    if ((freeCreated + takenCreated) % LINE_LENGTH == 0)
                    {
                        seatScheme.Children.Add(line = new WrapPanel());
                        line.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                    line.Children.Add(new freeSeat(Convert.ToInt32(freeSeats.Rows[freeCreated][0]), Convert.ToInt32(freeSeats.Rows[freeCreated++][1]), addReserv));
                }

                while (takenCreated != takenSeats.Rows.Count)
                {
                    if ((freeCreated + takenCreated) % LINE_LENGTH == 0)
                    {
                        seatScheme.Children.Add(line = new WrapPanel());
                        line.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                    line.Children.Add(new takenSeat());
                    takenCreated++;
                }
            }
        }

        private void addReserv(object sender, MouseButtonEventArgs e)
        {
            ((freeSeat)sender).clicked();
            if (((freeSeat)sender).isItActive())
            {
                ((freeSeat)sender).setSource("./Imgs/freeSeatClicked.png");
                reservedSeats.Rows.Add(((freeSeat)sender).getIdSeat());
            }
            else
            {
                ((freeSeat)sender).setSource("./Imgs/freeSeatUnclicked.png");
                DataRow toErase = null;
                foreach (DataRow row in reservedSeats.Rows) if (Convert.ToInt32(row[0]) == ((freeSeat)sender).getIdSeat()) toErase = row;
                if (toErase != null) reservedSeats.Rows.Remove(toErase);
            }
            if (reservedSeats.Rows.Count > 0) resNow.Visibility = Visibility.Visible; else resNow.Visibility = Visibility.Hidden;
        }

        private void resNow_Click(object sender, RoutedEventArgs e)
        {
            string idSeans = Manage.dbConSingle("SELECT s.ID_SEANS " +
                                                "FROM WSI_CINEMA_SEANS s " +
                                                "JOIN WSI_CINEMA_REPERTUAR   r ON(s.ID_REPERTUAR = r.ID_REPERTUAR) " +
                                                "WHERE s.ID_FILM = " + idFilm + " AND DATE_FORMAT(r.TERMIN,'%d.%m.%y') = '" + termin.SelectedValue + "' " +
                                                "AND s.CZAS_ROZPOCZECIA = '" + czas.SelectedValue + "' AND r.NR_SALA = " + sala.SelectedValue).ToString();
            foreach (DataRow row in reservedSeats.Rows) Manage.dbConNonQ("INSERT INTO WSI_CINEMA_REZERWACJA VALUES ( null, " + idUser + ", " + idSeans + ", " + row[0] + ")");
            this.Close();
            new wiadomosc("Dodano rezerwację.").ShowDialog();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }


    public class freeSeat : Image
    {
        private int idSeat;
        private int nrSeat;
        private bool isActive;

        public freeSeat(int idSeat, int nrSeat, Action<object, MouseButtonEventArgs> click)
        {
            this.idSeat = idSeat;
            this.nrSeat = nrSeat;
            Width = Height = 60;
            isActive = false;
            Margin = new Thickness(10, 10, 0, 0);
            Opacity = 0.5;
            Source = new BitmapImage(new Uri("./Imgs/freeSeatUnclicked.png", UriKind.RelativeOrAbsolute));
            IsMouseDirectlyOverChanged += anim;
            MouseLeftButtonUp += new MouseButtonEventHandler(click);
        }

        public void clicked()
        {
            isActive = !isActive;
        }

        public bool isItActive()
        {
            return isActive;
        }

        public void setSource(string uri)
        {
            Source = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        public int getIdSeat()
        {
            return idSeat;
        }

        public int getNrSeat()
        {
            return nrSeat;
        }

        private void anim(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((freeSeat)sender).Opacity < 1) ((freeSeat)sender).Opacity = 1; else ((freeSeat)sender).Opacity = 0.5;
        }
    }

    public class takenSeat : Image
    {
        public takenSeat()
        {
            Width = Height = 60;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = new Thickness(10, 10, 0, 0);
            Source = new BitmapImage(new Uri("./Imgs/takenSeat.png", UriKind.RelativeOrAbsolute));
            MouseLeftButtonUp += dontClick;
        }

        private void dontClick(object sender, MouseButtonEventArgs e)
        {
            new wiadomosc("To miejsce jest zajęte, wybierz inne.").ShowDialog();
        }
    }
}