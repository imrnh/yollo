using Npgsql;
using DotNetEnv;

public class ACreateGenre
{

    private string _connectionString = Env.GetString("CONNECTION_STRING");
    public List<string> GetAllGenreNames()
    {

        List<string> genreNames = new List<string>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT name FROM genre", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string genreName = reader.GetString(0);
                        genreNames.Add(genreName);
                    }
                }
            }
        }

        return genreNames;
    }


    public FunctionResponse InsertGenre(string name)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                // //get all the genre names
                // List<string> all_existing_genres = GetAllGenreNames();

                // //checking if the name exists in the db.
                // bool exists = all_existing_genres.Contains(name);

                // if (exists)
                // {
                //     return false;
                // }

                try
                {
                    command.CommandText = "INSERT INTO genre (name) VALUES (@name)";
                    command.Parameters.AddWithValue("name", name);

                    command.ExecuteNonQuery();

                    return new FunctionResponse(true, "New genre created");
                }catch(Exception e){
                    return new FunctionResponse(false, e.Message);
                }
            }
            connection.Close();
        }
    }

}