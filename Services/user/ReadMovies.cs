using Npgsql;
using DotNetEnv;


public class ReadMoviesService
{

    private List<int> FetchMovieGenres(int id, NpgsqlConnection connection)
    {
        string sql = "SELECT * FROM movie_genres WHERE movie_id = @movieId";

        List<int> genre_ids = new List<int>();

        using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@movieId", id);

            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int genreId = reader.GetInt32(reader.GetOrdinal("genre_id"));
                    genre_ids.Add(genreId);
                }
            }
        }

        return genre_ids;
    }

    private List<int> FetchMoviePublishers(int id, NpgsqlConnection connection)
    {
        string sql = "SELECT * FROM movie_publishers WHERE movie_id = @movieId";

        List<int> publishers_ids = new List<int>();

        using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@movieId", id);

            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int publihser_id = reader.GetInt32(reader.GetOrdinal("publisher_id"));
                    publishers_ids.Add(publihser_id);
                }
            }
        }

        return publishers_ids;
    }




    public List<MovieModel> ReadMovies(int limit = 100)
    {

        DotNetEnv.Env.Load();
        string _connectionString = Env.GetString("CONNECTION_STRING");

        List<MovieModel> movie_list = new List<MovieModel>();

        int _limit_counter = 1;

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            // Read the data from the "movie" table
            using (var command = new NpgsqlCommand("SELECT * FROM movie", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Read the values from the columns
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        string title = reader.GetString(reader.GetOrdinal("title"));
                        string description = reader.GetString(reader.GetOrdinal("description"));
                        DateTime publishedAt = reader.GetDateTime(reader.GetOrdinal("published_at"));
                        int ageLimit = reader.GetInt32(reader.GetOrdinal("age_limit"));
                        string bannerUrl = reader.GetString(reader.GetOrdinal("banner_url"));
                        List<string> movieFiles = (List<string>)reader.GetValue(reader.GetOrdinal("movie_files"));
                        int numberOfEpisodes = reader.GetInt32(reader.GetOrdinal("no_of_episodes"));
                        bool isSeries = reader.GetBoolean(reader.GetOrdinal("isSeries"));

                        //fetch genres
                        List<int> genres = FetchMovieGenres(id, connection);

                        //fetch publishers
                        List<int> publishers = FetchMoviePublishers(id, connection);

                        MovieModel new_movie = new MovieModel(id, title, description, genres, publishers, publishedAt, ageLimit, bannerUrl, movieFiles, numberOfEpisodes, isSeries);
                        movie_list.Add(new_movie);
                    }
                }
            }
            connection.Close();
        }


        return movie_list;
    }
}