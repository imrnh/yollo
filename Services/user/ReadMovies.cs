using Npgsql;
using DotNetEnv;
using System.Linq;


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
                            var movie_files_array = (string[])reader["movie_files"];
                            List<string> movie_files = movie_files_array.ToList();
                            int NumberOfEpisodes = (int)reader["no_of_episodes"];
                            bool IsSeries = (bool)reader["isSeries"];
                            string slug = (string)reader["movie_slug"];

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
                            movie.slug = slug; //setting the slug

                            Dictionary<string, dynamic> response_data = new Dictionary<string, dynamic>();

                            response_data["movie"] = movie;
                            response_data["genres"] = genrs;
                            response_data["publishers"] = publishers;

                            return new FunctionResponse(true, response_data);

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

    private List<int> FetchMovieGenres(int id)
    {
        List<int> genre_ids = new List<int>();
        using (var connection = new NpgsqlConnection(this._connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM movie_genres WHERE movie_id = @movieId";



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
            connection.Close();
        }

        return genre_ids;
    }

    private List<int> FetchMoviePublishers(int id)
    {


        List<int> publishers_ids = new List<int>();


        using (var connection = new NpgsqlConnection(this._connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM movie_publishers WHERE movie_id = @movieId";

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
                        List<string> movieFiles = new List<string>((string[])reader.GetValue(reader.GetOrdinal("movie_files")));
                        int numberOfEpisodes = reader.GetInt32(reader.GetOrdinal("no_of_episodes"));
                        bool isSeries = reader.GetBoolean(reader.GetOrdinal("isSeries"));
                        string slug = reader.GetString(reader.GetOrdinal("movie_slug"));

                        //fetch genres
                        List<int> genres = FetchMovieGenres(id);

                        //fetch publishers
                        List<int> publishers = FetchMoviePublishers(id);


                        MovieModel new_movie = new MovieModel(id, title, description, genres, publishers, publishedAt, ageLimit, bannerUrl, movieFiles, numberOfEpisodes, isSeries);
                        new_movie.slug = slug;

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


    /****
     * 
     * Read all the genres.
     * 
     * ****/

    public List<GenreModel> ReadGenres()
    {

        List<GenreModel> genres = new List<GenreModel>();

        using (var connection = new NpgsqlConnection(this._connectionString))
        {
            connection.Open();

            var query = "SELECT * FROM genre";
            using (var command = new NpgsqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var gname = reader.GetString(1);

                        GenreModel gm = new GenreModel(id, gname);
                        genres.Add(gm);
                    }
                }
            }

            connection.Close();
        }

        return genres;
    }


    /****
    * 
    * Read name of the genre from genre id.
    * 
    * ****/

    public FunctionResponse GenreNameFromId(int[] g_ids)
    {
        using (var connection = new NpgsqlConnection(this._connectionString))
        {
            connection.Open();
            List<GenreModel> genres = new List<GenreModel>();

            foreach (var gid in g_ids)
            {
                var query = "SELECT * FROM genre where id=@GID";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("GID", gid);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var gname = reader.GetString(1);
                            genres.Add(new GenreModel(id, gname));
                        }

                    }
                }
            }

            return new FunctionResponse(true, genres);

            connection.Close();
        }

        return new FunctionResponse(false, null);
    }

}