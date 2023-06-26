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
}