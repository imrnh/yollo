using Npgsql;
using DotNetEnv;


public class ReadMoviesService
{

    private readonly string _connectionString;
    public ReadMoviesService()
    {
        DotNetEnv.Env.Load();
        _connectionString = Env.GetString("CONNECTION_STRING");
    }


    public FunctionResponse SingleMovie(string movie_slug = "", int movie_id = -1)
    {
        using (var connection = new NpgsqlConnection(this._connectionString))
        {
            connection.Open();

            var query = "";

            if (movie_id != -1)
            {
                query = "SELECT * FROM movie WHERE id = @MovieId";
            }

            else
            {
                query = "SELECT * FROM movie WHERE movie_slug = @MovieSlug";
            }



            using (var command = new NpgsqlCommand(query, connection))
            {

                if (movie_id != -1)
                {
                    command.Parameters.AddWithValue("@MovieId", movie_id);
                }

                else
                {
                    command.Parameters.AddWithValue("@MovieSlug", movie_slug);
                }



                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            int Id = (int)reader["id"];
                            string Title = (string)reader["title"];
                            string MovieSlug = (string)reader["movie_slug"];
                            string Description = (string)reader["description"];
                            DateTime PublishedAt = (DateTime)reader["published_at"];
                            int AgeLimit = (int)reader["age_limit"];
                            string BannerUrl = (string)reader["banner_url"];
                            List<string> movie_files = new List<string>((string[])reader["movie_files"]);
                            int NumberOfEpisodes = (int)reader["no_of_episodes"];
                            bool IsSeries = (bool)reader["isSeries"];

                            //fetch genres
                            List<GenreModel> genrs = new AMovieServices().GetGenresByMovieId(Id).value;
                            List<int> genr_ids = new List<int>();

                            foreach (var gnr in genrs)
                            {
                                genr_ids.Add(gnr.Id);
                            }


                            //fetch publishers.

                            List<PublisherModel> publishers = new AMovieServices().GetPublishersByMovieId(Id).value;
                            List<int> publisher_ids = new List<int>();

                            foreach (var pbr in publishers)
                            {
                                publisher_ids.Add(pbr.id);
                            }

                            MovieModel movie = new MovieModel(Id, Title, Description, genr_ids, publisher_ids, PublishedAt, AgeLimit, BannerUrl, movie_files, NumberOfEpisodes, IsSeries);

                            return new FunctionResponse(true, movie);

                        }
                    }
                }
                catch (Exception e)
                {
                    return new FunctionResponse(false, e.Message);
                }

                return new FunctionResponse(false, null);
            }
        }
    }

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



        List<MovieModel> movie_list = new List<MovieModel>();

        int _limit_counter = 1;

        using (var connection = new NpgsqlConnection(this._connectionString))
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



    /***
    
        - Search movie.

    ***/

    public List<MovieModel> SearchMovies(string searchKeyword)
    {
        List<MovieModel> movies = new List<MovieModel>();
        using (var connection = new NpgsqlConnection(this._connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM movie WHERE title ILIKE @searchKeyword";
            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("searchKeyword", $"%{searchKeyword}%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string movieSlug = Convert.ToString(reader["movie_slug"]);

                        movies.Add(SingleMovie(movieSlug).value);
                    }
                }
            }
        }
        return movies;
    }

}