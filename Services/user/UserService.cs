using DotNetEnv;
using Npgsql;


public class UserService
{

    private readonly string connectionString;
    public UserService()
    {
        DotNetEnv.Env.Load();
        this.connectionString = Env.GetString("CONNECTION_STRING");
    }

    public UserOutputModel GetUser(int my_id)
    {

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT id, full_name, email, dob, created_at FROM users where id=@ID";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("ID", my_id);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int userId = my_id;
                        string fullName = reader.GetString(reader.GetOrdinal("full_name"));
                        string email = reader.GetString(reader.GetOrdinal("email"));
                        DateTime dob = reader.GetDateTime(reader.GetOrdinal("dob"));
                        DateTime createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));

                        UserOutputModel user = new UserOutputModel(userId, fullName, email, dob, createdAt);
                        return user;
                    }
                }
            }
        }

        return null;
    }

}