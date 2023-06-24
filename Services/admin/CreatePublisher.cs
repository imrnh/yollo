using Npgsql;
using DotNetEnv;

public class ACreatePublisher
{

    private string _connectionString = Env.GetString("CONNECTION_STRING");
    public FunctionResponse InsertPublisher(string name)
    {


        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                // Prepare the SQL statement with parameters
                command.CommandText = "INSERT INTO publisher (name) VALUES (@name)";
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