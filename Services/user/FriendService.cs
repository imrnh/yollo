
using DotNetEnv;
using Npgsql;
namespace Netflix.Services.user;


public class Friend
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}


public class FriendService
{
    private readonly string connectionString;
    public FriendService()
    {
        DotNetEnv.Env.Load();
        this.connectionString = Env.GetString("CONNECTION_STRING");
    }


    /*
        - Search a user with matching email or almost similar fullname.
    */

    public FunctionResponse SearchUser(string email = "", string fullname = "")
    {

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT id, full_name, email FROM users WHERE (email = @Email OR full_name LIKE @Name) AND isAdmin = false", connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Name", $"%{fullname}%");

                using (var reader = command.ExecuteReader())
                {
                    var friends = new List<Friend>();

                    while (reader.Read())
                    {
                        Friend friend = new Friend
                        {
                            Id = (int)reader["id"],
                            FullName = (string)reader["full_name"],
                            Email = (string)reader["email"]
                        };

                        friends.Add(friend);
                    }

                    return new FunctionResponse(true, friends);
                }
            }
        }

        return new FunctionResponse(false, null);
    }
}