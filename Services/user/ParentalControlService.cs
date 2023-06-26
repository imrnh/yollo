using DotNetEnv;
using Npgsql;
namespace Netflix.Services.user;




public class ParentalControlService
{

    private readonly string connectionString;
    public ParentalControlService()
    {
        DotNetEnv.Env.Load();
        this.connectionString = Env.GetString("CONNECTION_STRING");
    }


    public FunctionResponse ToggleParentalControl(int userId, bool value, string pctrl_pass, int age_limit = -1, int[] allowed_genres_list = null)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                string query = "UPDATE users SET parental_control_active = @value WHERE id = @userId";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("value", value);
                    command.Parameters.AddWithValue("userId", userId);

                    if (value)
                    { // if setting parantal control.
                        FunctionResponse param_resp = this.SetParentalControlParams(userId, pctrl_pass, age_limit);
                        FunctionResponse grn_resp = this.SetParentalControlGenres(userId, allowed_genres_list);

                        if (!param_resp.status)
                            return param_resp.value;
                        if (!grn_resp.status)
                            return grn_resp.value;

                        command.ExecuteNonQuery();
                        return new FunctionResponse(true, "Parental control activated");
                    }
                    else
                    {
                        //ask password to deactivate. if password not given, return nothing and do not execute.
                        string db_pass = "";
                        FunctionResponse fetch_password_response = this.ReadControlPassword(userId);
                        if (!fetch_password_response.status)
                        {
                            return new FunctionResponse(false, $"Error reading passwod. {fetch_password_response.value}");
                        }

                        db_pass = fetch_password_response.value;

                        //hashing operation here.
                        if (db_pass.Equals(pctrl_pass))
                        {
                            command.ExecuteNonQuery();
                            return new FunctionResponse(true, "Parental control de-activated");
                        }
                        else
                        {
                            return new FunctionResponse(false, "Invalid password");
                        }

                    }

                }
            }
            catch (Exception e)
            {
                return new FunctionResponse(false, e.Message);
            }
        }

    }


    private FunctionResponse SetParentalControlParams(int userId, string pctrlPassword, int ageLimit)
    {
        try
        {
            using (var connection = new NpgsqlConnection(this.connectionString))
            {
                connection.Open();

                string query = "INSERT INTO parental_control (user_id, control_password, age_limit) VALUES (@userId, @pctrlPassword, @ageLimit)" +
                               " ON CONFLICT (user_id) DO UPDATE SET control_password = @pctrlPassword, age_limit = @ageLimit";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);
                    command.Parameters.AddWithValue("pctrlPassword", pctrlPassword);
                    command.Parameters.AddWithValue("ageLimit", ageLimit);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return new FunctionResponse(true, "Parental control parameters saved successfully.");
                    }
                    else
                    {
                        return new FunctionResponse(false, "Failed to save parental control parameters.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return new FunctionResponse(false, ex.Message);
        }
    }

    private FunctionResponse SetParentalControlGenres(int userId, int[] allowedGenres)
    {
        try
        {
            using (var connection = new NpgsqlConnection(this.connectionString))
            {
                connection.Open();

                // Delete existing records for the user_id
                string deleteQuery = "DELETE FROM prntlctrl_allowed_genres WHERE user_id = @userId";
                using (var deleteCommand = new NpgsqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("userId", userId);
                    deleteCommand.ExecuteNonQuery();
                }

                // Insert new records for the user_id and allowedGenres
                string insertQuery = "INSERT INTO prntlctrl_allowed_genres (user_id, genre_id) VALUES (@userId, @genreId)";
                using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("userId", userId);
                    insertCommand.Parameters.Add("genreId", NpgsqlTypes.NpgsqlDbType.Integer);

                    foreach (int genreId in allowedGenres)
                    {
                        insertCommand.Parameters["genreId"].Value = genreId;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                return new FunctionResponse(true, "Allowed genres added successfully.");
            }
        }
        catch (Exception ex)
        {
            return new FunctionResponse(false, ex.Message);
        }
    }



    private FunctionResponse ReadControlPassword(int userId)
    {
        try
        {
            using (var connection = new NpgsqlConnection(this.connectionString))
            {
                connection.Open();

                string query = "SELECT control_password FROM parental_control WHERE user_id = @userId";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new FunctionResponse(true, reader["control_password"].ToString());
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return new FunctionResponse(false, ex.Message);
        }
        return new FunctionResponse(false, "Some error we have no idea about but asp think it can access this area of the code. So I had to add this return statement.");
    }



    /****
    
        - Read parental control age_limit
    
    *****/

    public int ReadParentalControlAgeLimit(int userId)
    {
        try
        {
            using (var connection = new NpgsqlConnection(this.connectionString))
            {
                connection.Open();

                string query = "SELECT age_limit FROM parental_control WHERE user_id = @userId";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int ageLimit = Convert.ToInt32(reader["age_limit"]);

                            return ageLimit;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            return -1;
        }

        return -1;
    }


    public List<int> ReadParentalControlGenres(int userId)
    {
        List<int> allowedGenres = new List<int>();

        try
        {
            using (var connection = new NpgsqlConnection(this.connectionString))
            {
                connection.Open();

                string query = "SELECT genre_id FROM prntlctrl_allowed_genres WHERE user_id = @userId";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int genreId = Convert.ToInt32(reader["genre_id"]);
                            allowedGenres.Add(genreId);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            return null;
        }

        return allowedGenres;
    }



    public bool CheckMovieGenres(List<int> allowedGenres, MovieModel movie)
    {
        foreach (int genreId in movie.genres)
        {
            if (!allowedGenres.Contains(genreId))
            {
                return false; // Genre not allowed
            }
        }

        return true;
    }

    public bool CheckAgeLimit(int age_limit, MovieModel movie)
    {
        return movie.age_limit <= age_limit;
    }


    public bool CheckParentalControlActive(int userId)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT parental_control_active FROM users WHERE id = @userId";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);

                    var result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToBoolean(result);
                    }
                }
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false; // Default value if no record found or error occurred
    }


    public FunctionResponse FilterWithParentalControl(List<MovieModel> moviesToFilter, int userId)
    {
        if (!this.CheckParentalControlActive(userId))
            return new FunctionResponse(true, moviesToFilter);

        List<MovieModel> filteredMovies = new List<MovieModel>();

        int allowed_age_limit = this.ReadParentalControlAgeLimit(userId);
        List<int> allowed_genres = this.ReadParentalControlGenres(userId);

        foreach (var movie in moviesToFilter)
        {
            if (CheckMovieGenres(allowed_genres, movie) && CheckAgeLimit(allowed_age_limit, movie))
            {
                filteredMovies.Add(movie);
            }
        }

        return new FunctionResponse(true, filteredMovies);
    }


    public FunctionResponse FilterWithParentalControl(MovieModel movie, int userId)
    {
        if (!this.CheckParentalControlActive(userId))
            return new FunctionResponse(true, movie);

        MovieModel filteredMovies;

        int allowed_age_limit = this.ReadParentalControlAgeLimit(userId);
        List<int> allowed_genres = this.ReadParentalControlGenres(userId);

        if (CheckMovieGenres(allowed_genres, movie) && CheckAgeLimit(allowed_age_limit, movie))
        {
            filteredMovies = movie;
            return new FunctionResponse(true, filteredMovies);
        }
        else
        {
            return new FunctionResponse(false, "Content blocked due to parental control.");
        }
    }
}