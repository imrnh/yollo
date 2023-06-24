using Npgsql;
using DotNetEnv;

public class NetflixDbAccessModel
{
    private readonly string _connectionString;

    public NetflixDbAccessModel()
    {
        DotNetEnv.Env.Load();
        _connectionString = Env.GetString("CONNECTION_STRING");
        // _connectionString = "Host=localhost;Port=5432;Database=ott_platform;Username=postgres;Password=password";
    }


    //
    public List<GenreModel> ReadGenres()
    {

        List<GenreModel> genres = new List<GenreModel>();

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

                        GenreModel gm = new GenreModel(id, gname);
                        genres.Add(gm);
                    }
                }
            }

            connection.Close();
        }

        return genres;
    }
}
