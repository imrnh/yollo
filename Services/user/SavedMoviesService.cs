using System.Security.Claims;
using System.Security.Cryptography;
using DotNetEnv;
using Npgsql;



namespace Netflix.Services.user;
public class SavedMoviesService
{

    private string connectionString;
    public SavedMoviesService()
    {
        DotNetEnv.Env.Load();
        this.connectionString = Env.GetString("CONNECTION_STRING");
    }


    public FunctionResponse LoadWatchHistory(int user_id)
    {
        List<int> movie_ids = new List<int>();
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            try
            {
                string query = "SELECT movie_id FROM watch_history WHERE user_id = @userId";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", user_id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int movieId = reader.GetInt32(0);
                            movie_ids.Add(movieId);
                        }
                        return new FunctionResponse(true, movie_ids);
                    }
                }
            }
            catch (Exception e)
            {
                return new FunctionResponse(false, e.Message);
            }
        }
    }


    public FunctionResponse LoadWatchLater(int user_id)
    {
        List<int> movie_ids = new List<int>();
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            try
            {
                string query = "SELECT movie_id FROM watch_later WHERE user_id = @userId ORDER BY created_at";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", user_id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int movieId = reader.GetInt32(0);
                            movie_ids.Add(movieId);
                        }
                        return new FunctionResponse(true, movie_ids);
                    }
                }
            }
            catch (Exception e)
            {
                return new FunctionResponse(false, e.Message);
            }
        }
    }



    /**
        - Check if the watch_history and watch_later for a user is publicly visible or not.

    
    **/

    public bool[] WHL_Public(int userId){

        bool[] publicStatusArray = new bool[2];
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT wh_public, wl_public FROM whpublic WHERE user_id = @userId";
            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("userId", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bool whPublic = reader.GetBoolean(0);
                        bool wlPublic = reader.GetBoolean(1);

                        
                        publicStatusArray[0] = whPublic;
                        publicStatusArray[1] = wlPublic;

                        
                    }
                }
            }
        }

        return publicStatusArray;
    }

}