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
    public class Manage
    {
        public static void hudCreate(WrapPanel wp, Action<object, RoutedEventArgs> action, string aButtCon, string query)
        {
            DataTable queryResult = new DataTable();
            dbConTab(query).Fill(queryResult);
            wp.Children.Clear();

            if (queryResult.Rows.Count == 0)
            {
                Label notFound = new Label();
                notFound.Content = "Brak elementow do wyswietlenia";
                notFound.FontSize = 32;
                wp.Children.Add(notFound);
            }
            else
            {
                /*
                Thread firstThread = new Thread(()=>
                {
                    for (int i = 0; i < queryResult.Rows.Count; i+=2)
                    {
                        DataRow row = queryResult.Rows[i];
                        ActionButton aButt = new ActionButton(row[0].ToString(), aButtCon, action);
                        MoreButton mButt = new MoreButton(row[1].ToString(), row[2].ToString());

                        Movie movie = new TMDbClient("7e8f60e325cd06e164799af1e317d7a7").GetMovieAsync(Convert.ToInt32(row[1]), "pl", MovieMethods.Credits).Result;
                        MovieIcon icon = new MovieIcon(mButt, aButt, "https://image.tmdb.org/t/p/w500/" + movie.PosterPath);

                        wp.Children.Add(icon);
                    }
                });
                Thread secondThread = new Thread(() =>
                {
                    for (int i = 1; i < queryResult.Rows.Count; i += 2)
                    {
                        DataRow row = queryResult.Rows[i];
                        ActionButton aButt = new ActionButton(row[0].ToString(), aButtCon, action);
                        MoreButton mButt = new MoreButton(row[1].ToString(), row[2].ToString());

                        Movie movie = new TMDbClient("7e8f60e325cd06e164799af1e317d7a7").GetMovieAsync(Convert.ToInt32(row[1]), "pl", MovieMethods.Credits).Result;
                        MovieIcon icon = new MovieIcon(mButt, aButt, "https://image.tmdb.org/t/p/w500/" + movie.PosterPath);

                        wp.Children.Add(icon);
                    }
                });
                firstThread.Start();
                secondThread.Start();
                */
                /*
                foreach (DataRow row in queryResult.Rows)
                {
                    ActionButton aButt = new ActionButton(row[0].ToString(), aButtCon, action);
                    MoreButton mButt = new MoreButton(row[1].ToString(), row[2].ToString());

                    Movie movie = new TMDbClient("7e8f60e325cd06e164799af1e317d7a7").GetMovieAsync(Convert.ToInt32(row[1]), "pl", MovieMethods.Credits).Result;
                    MovieIcon icon = new MovieIcon(mButt, aButt, "https://image.tmdb.org/t/p/w500/" + movie.PosterPath);

                    wp.Children.Add(icon);
                }*/
            }
        }

        public static MySqlDataAdapter dbConTab(string query)
        {
            return new MySqlDataAdapter(query, connection: new MySqlConnection("SERVER=lamp.ii.us.edu.pl;PORT=3306;DATABASE=ii302904;UID=ii302904;PASSWORD=Dzwonek92!;"));
        }

        public static object dbConSingle(string query)
        {
            MySqlConnection connection = new MySqlConnection("SERVER=lamp.ii.us.edu.pl;PORT=3306;DATABASE=ii302904;UID=ii302904;PASSWORD=Dzwonek92!;");
            connection.Open();
            object toReturn = (new MySqlCommand(query, connection)).ExecuteScalar();
            connection.Close();
            return toReturn;
        }

        public static void dbConNonQ(string query)
        {
            MySqlConnection connection = new MySqlConnection("SERVER=lamp.ii.us.edu.pl;PORT=3306;DATABASE=ii302904;UID=ii302904;PASSWORD=Dzwonek92!;");
            connection.Open();
            (new MySqlCommand(query, connection)).ExecuteNonQuery();
            connection.Close();
        }
    }
}
