using DotNetEnv;
using Npgsql;
namespace Netflix.Services.user;

public class MovieFilteringService
{
    private readonly string connectionString;
    public MovieFilteringService()
    {
        DotNetEnv.Env.Load();
        this.connectionString = Env.GetString("CONNECTION_STRING");
    }


    
    public static FunctionResponse GetCommonMovies(List<MovieModel> list1, List<MovieModel> list2)
    {
        List<MovieModel> commonMovies = new List<MovieModel>();

        foreach (MovieModel movie1 in list1)
        {
            foreach (MovieModel movie2 in list2)
            {
                if (movie1.id == movie2.id)
                {
                    commonMovies.Add(movie1);
                    break; // Found a common movie, move to the next movie in list1
                }
            }
        }

        return new FunctionResponse(true, commonMovies);
    }

    public FunctionResponse FilterByGenre(int genre_id)
    {
        List<MovieModel> movies = new List<MovieModel>();
        
        
        using (var connection = new NpgsqlConnection(this.connectionString))
        {
            connection.Open();
        
            using (var command = new NpgsqlCommand("select * from movie where id = (select movie_id from movie_genres where genre_id = @GenreId)", connection))
            {
                command.Parameters.AddWithValue("GenreId", genre_id);
                using (var reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["id"]);
                            string title = Convert.ToString(reader["title"]);
                            string description = Convert.ToString(reader["description"]);
                            string banner_url = Convert.ToString(reader["banner_url"]);
                            List<string> movie_files = new List<string>((string[])reader["movie_files"]);
                            bool isSeries = Convert.ToBoolean(reader["isseries"]);
                            int age_limit = Convert.ToInt32(reader["age_limit"]);
                            DateTime published_at = Convert.ToDateTime(reader["published_at"]);
                            int no_of_eps = Convert.ToInt32(reader["no_of_episodes"]);
                        
                        
                            //fetch all the genres of given movie.
                            List<GenreModel> genres = new List<GenreModel>();
                            List<int> genre_ids = new List<int>();
                            FunctionResponse genre_response = new AMovieServices().GetGenresByMovieId(id);
                            if (!genre_response.status)
                                return new FunctionResponse(false, null);

                            genres = genre_response.value;

                            foreach (var gnr in genres)
                            {
                                genre_ids.Add(gnr.Id);
                            }
                        
                            //fetch all the publishers of given movie.
                            List<PublisherModel> publishers = new List<PublisherModel>();
                            List<int> publishers_ids = new List<int>();
                            FunctionResponse publishers_response = new AMovieServices().GetPublishersByMovieId(id);
                            if (!publishers_response.status)
                                return new FunctionResponse(false, null);

                            publishers = publishers_response.value;
                        
                            foreach (var pbr in publishers)
                            {
                                publishers_ids.Add(pbr.id);
                            }
                        

                            MovieModel movie = new MovieModel(id, title, description, genre_ids, publishers_ids, published_at, age_limit, banner_url, movie_files, no_of_eps, isSeries);
                            movies.Add(movie);
                        }
                        return new FunctionResponse(true, movies);
                    }
                    catch (Exception e)
                    {
                        return new FunctionResponse(false, e.Message);
                    }
                }
            }
        }
        
        return new FunctionResponse(false, null);
    }
    
    
    public FunctionResponse FilterByPublisher(int publisher_id)
    {
        List<MovieModel> movies = new List<MovieModel>();
        
        
        using (var connection = new NpgsqlConnection(this.connectionString))
        {
            connection.Open();
        
            using (var command = new NpgsqlCommand("select * from movie where id = (select movie_id from movie_publishers where publisher_id = @PubId)", connection))
            {
                command.Parameters.AddWithValue("PubId", publisher_id);
                using (var reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["id"]);
                            string title = Convert.ToString(reader["title"]);
                            string description = Convert.ToString(reader["description"]);
                            string banner_url = Convert.ToString(reader["banner_url"]);
                            List<string> movie_files = new List<string>((string[])reader["movie_files"]);
                            bool isSeries = Convert.ToBoolean(reader["isseries"]);
                            int age_limit = Convert.ToInt32(reader["age_limit"]);
                            DateTime published_at = Convert.ToDateTime(reader["published_at"]);
                            int no_of_eps = Convert.ToInt32(reader["no_of_episodes"]);
                        
                        
                            //fetch all the genres of given movie.
                            List<GenreModel> genres = new List<GenreModel>();
                            List<int> genre_ids = new List<int>();
                            FunctionResponse genre_response = new AMovieServices().GetGenresByMovieId(id);
                            if (!genre_response.status)
                                return new FunctionResponse(false, null);

                            genres = genre_response.value;

                            foreach (var gnr in genres)
                            {
                                genre_ids.Add(gnr.Id);
                            }
                        
                            //fetch all the publishers of given movie.
                            List<PublisherModel> publishers = new List<PublisherModel>();
                            List<int> publishers_ids = new List<int>();
                            FunctionResponse publishers_response = new AMovieServices().GetPublishersByMovieId(id);
                            if (!publishers_response.status)
                                return new FunctionResponse(false, null);

                            publishers = publishers_response.value;
                        
                            foreach (var pbr in publishers)
                            {
                                publishers_ids.Add(pbr.id);
                            }
                        

                            MovieModel movie = new MovieModel(id, title, description, genre_ids, publishers_ids, published_at, age_limit, banner_url, movie_files, no_of_eps, isSeries);
                            movies.Add(movie);
                        }
                        return new FunctionResponse(true, movies);
                    }
                    catch (Exception e)
                    {
                        return new FunctionResponse(false, e.Message);
                    }
                }
            }
        }
        
        return new FunctionResponse(false, null);
    }
}