
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



    public FunctionResponse MakeFriend(int my_id, int friend_id)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO friend (user1, user2) VALUES (@User1Id, @User2Id)";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("User1Id", my_id);
                command.Parameters.AddWithValue("User2Id", friend_id);

                try
                {
                    command.ExecuteNonQuery();
                    return new FunctionResponse(true, "Friend created succesfully");
                }
                catch (Exception e)
                {
                    return new FunctionResponse(false, e.Message);
                }

            }
        }
    }


    public FunctionResponse LoadFriend(int my_id)
    {
        List<int> friends = new List<int>();
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT user2 FROM friend WHERE user1 = @User1Id";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("User1Id", my_id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int user2Id = reader.GetInt32(0);
                        friends.Add(user2Id);
                    }

                    return new FunctionResponse(true, GetUserDetails(friends));
                }
            }
        }
        return new FunctionResponse(false, null);
    }


    List<User> GetUserDetails(List<int> ids)
    {
        List<User> users = new List<User>();

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT full_name, email FROM users WHERE id = ANY(@Ids)";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("Ids", ids.ToArray());

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string fullName = reader.GetString(0);
                        string email = reader.GetString(1);
                        users.Add(new User { FullName = fullName, Email = email });
                    }
                }
            }
        }

        return users;
    }

}


public class User
{
    public string FullName { get; set; }
    public string Email { get; set; }
}