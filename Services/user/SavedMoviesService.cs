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




    public FunctionResponse SavedMovieToWatchHistory(int userId, int movieId)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            try
            {
                string query = "INSERT INTO watch_history (user_id, movie_id) VALUES (@userId, @movieId)";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);
                    command.Parameters.AddWithValue("movieId", movieId);

                    int rowsAffected = command.ExecuteNonQuery();
                    return new FunctionResponse(true, "Movie added to watch history");
                }
            }
            catch (Exception e)
            {
                return new FunctionResponse(false, e.Message);
            }
        }
    }


    public FunctionResponse SavedMovieToWatchLater(int userId, int movieId)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            try
            {
                string query = "INSERT INTO watch_later (user_id, movie_id) VALUES (@userId, @movieId)";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);
                    command.Parameters.AddWithValue("movieId", movieId);

                    int rowsAffected = command.ExecuteNonQuery();
                    return new FunctionResponse(true, "Movie added to watch later");
                }
            }
            catch (Exception e)
            {
                return new FunctionResponse(false, e.Message);
            }
        }
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



    public FunctionResponse RemoveFromWatchHistory(int userId, int movieId)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            try
            {
                string query = "DELETE FROM watch_history WHERE user_id = @userId AND movie_id = @movieId";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);
                    command.Parameters.AddWithValue("movieId", movieId);

                    int rowsAffected = command.ExecuteNonQuery();
                    return new FunctionResponse(true, "Movie Deleted");
                }
            }
            catch (Exception e)
            {
                return new FunctionResponse(false, e.Message);
            }
        }
    }



        public FunctionResponse RemoveFromWatchLater(int userId, int movieId)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            try
            {
                string query = "DELETE FROM watch_later WHERE user_id = @userId AND movie_id = @movieId";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);
                    command.Parameters.AddWithValue("movieId", movieId);

                    int rowsAffected = command.ExecuteNonQuery();
                    return new FunctionResponse(true, "Movie deleted");
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

    public bool[] Check_WHL_Public(int userId)
    {

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



    public FunctionResponse Trigger_WHL_Public(int userId, bool newValue, int flag)
    {
        //invalid flag check
        if (flag == -1)
            return new FunctionResponse(false, "Invalid flag");



        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();



            bool recordExists = WHLTriggerHelper.CheckRecordExists(connection, userId);
            if (!recordExists)
            {
                WHLTriggerHelper.InsertDefaultRecord(connection, userId);
            }

            string columnName = (flag == 0) ? "wh_public" : "wl_public";
            string query = $"UPDATE whpublic SET {columnName} = @newValue WHERE user_id = @userId";
            try
            {
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("newValue", newValue);
                    command.Parameters.AddWithValue("userId", userId);

                    int rowsAffected = command.ExecuteNonQuery();

                }

                return new FunctionResponse(true, $"Triggered {columnName}");
            }
            catch (Exception e)
            {
                return new FunctionResponse(false, e.Message);
            }
        }

    }




}



class WHLTriggerHelper
{
    public static bool CheckRecordExists(NpgsqlConnection connection, int userId)
    {
        string query = "SELECT COUNT(*) FROM whpublic WHERE user_id = @userId";
        using (var command = new NpgsqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("userId", userId);
            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }
    }

    public static void InsertDefaultRecord(NpgsqlConnection connection, int userId)
    {
        string query = "INSERT INTO whpublic (user_id) VALUES (@userId)";
        using (var command = new NpgsqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("userId", userId);
            command.ExecuteNonQuery();
            Console.WriteLine("Default record inserted.");
        }
    }
}
