using Npgsql;

public class NetflixDbAccessModel
{
    private readonly string _connectionString;

    public NetflixDbAccessModel()
    {
        _connectionString = "Host=localhost;Port=5432;Database=ac_netflix;Username=postgres;Password=password";
    }


    //
    public List<GenereModel> ReadGenres()
    {

        List<GenereModel> genres = new List<GenereModel>();

        using (var connection = new NpgsqlConnection(_connectionString))
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

                        GenereModel gm = new GenereModel(id, gname);
                        genres.Add(gm);
                    }
                }
            }

            connection.Close();
        }

        return genres;
    }


    public List<MovieModel> ReadMovies(int limit=100)
    {

        List<MovieModel> movie_list = new List<MovieModel>();

        int _limit_counter = 1;

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            var query = "SELECT * FROM movie";
            using (var command = new NpgsqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try{
                            var id = reader.GetInt32(0);
                        var title = reader.GetString(1);
                        var description = reader.GetString(2);
                        var creation_date = reader.GetDateTime(3);
                        var publisher_id = reader.GetInt32(4);
                        var publishing_year = reader.GetInt32(5);
                        var banner_url = reader.GetString(6);

                        MovieModel movie = new MovieModel(id, title, description, publisher_id, publishing_year, banner_url);
                        movie_list.Add(movie);
                        }
                        catch(Exception e){
                            Console.WriteLine("Exception reading movies: " + e);
                        }

                        //do not return more than limits.
                        _limit_counter++;
                        if(_limit_counter > limit){
                            break;
                        }
                    }
                }
            }

            connection.Close();
        }

        return movie_list;
    }
}
