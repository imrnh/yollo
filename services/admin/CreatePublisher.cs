using Npgsql;
using DotNetEnv;

public class ACreatePublisher
{

    public static void InsertPublisher(string name)
    {
        string _connectionString = Env.GetString("CONNECTION_STRING");

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                // Prepare the SQL statement with parameters
                command.CommandText = "INSERT INTO publisher (name) VALUES (@name)";
                command.Parameters.AddWithValue("name", name);

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}