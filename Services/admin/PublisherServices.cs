using Npgsql;
using DotNetEnv;

public class APublisherServices
{
    private string _connectionString;
    public APublisherServices()
    {
        DotNetEnv.Env.Load();
        this._connectionString = Env.GetString("CONNECTION_STRING");
    }

    public List<PublisherModel> GetAllPublishers()
    {
        List<PublisherModel> publishers = new List<PublisherModel>();

        using (var connection = new NpgsqlConnection(this._connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT * FROM publisher", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int Id = Convert.ToInt32(reader["id"]);
                        string Name = reader["name"].ToString();

                        PublisherModel publisher = new PublisherModel(Id, Name);
                        publishers.Add(publisher);
                    }
                }
            }
        }

        return publishers;
    }

    public FunctionResponse InsertPublisher(string name)
    {


        using (var connection = new NpgsqlConnection(this._connectionString))
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