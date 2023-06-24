using Npgsql;
using DotNetEnv;

public class AGenreServices
{

    private string _connectionString;
    public AGenreServices()
    {
        DotNetEnv.Env.Load();
        this._connectionString = Env.GetString("CONNECTION_STRING");
    }

    public List<GenreModel> GetAllGenres()
    {
        List<GenreModel> genres = new List<GenreModel>();

        using (var connection = new NpgsqlConnection(this._connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT * FROM genre", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int Id = Convert.ToInt32(reader["id"]);
                        string Name = reader["name"].ToString();

                        

                        GenreModel genre = new GenreModel(Id, Name);
                        genres.Add(genre);
                    }
                }
            }
        }

        return genres;
    }


    public FunctionResponse InsertGenre(string name)
    {
        using (var connection = new NpgsqlConnection(this._connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                command.CommandText = "INSERT INTO genre (name) VALUES (@name)";
                command.Parameters.AddWithValue("name", name);

                try
                {
                    command.ExecuteNonQuery();
                    return new FunctionResponse(true, "New genre created");
                }
                catch (PostgresException pgexp)
                {
                    return new FunctionResponse(false, pgexp.MessageText);
                }
            }
            connection.Close();
        }
    }

}