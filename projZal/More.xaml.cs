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
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace projZal
{
    /// <summary>
    /// Interaction logic for More.xaml
    /// </summary>
    public partial class More : Window
    {
        private string url;
        public More(string id, string url)
        {
            InitializeComponent();

            this.url = url;
            Movie movie = new TMDbClient("7e8f60e325cd06e164799af1e317d7a7").GetMovieAsync(id, "pl", MovieMethods.Credits).Result;

            poster.Source = new BitmapImage(new Uri("https://image.tmdb.org/t/p/w500/" + movie.PosterPath));
            wndTitle.Content = movie.Title;
            length.Content = "Czas trwania: " + movie.Runtime + " minut";
            premDate.Content = "Data premiery: " + movie.ReleaseDate.ToString().Remove(11, 8);
            review.Text = movie.Overview;
        }

        private void trailerWnd_Click(object sender, RoutedEventArgs e)
        {
            TrailerWindow t = new TrailerWindow(wndTitle.Content.ToString(), url);
            t.ShowDialog();
        }

        private void image1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
