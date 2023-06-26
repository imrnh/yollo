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


    public bool ToggleParentalControl(int userId, bool value, string pctrl_pass, int age_limit = -1, int[] allowed_genres_list = null)
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

                    if(value){ // if setting parantal control.
                        SetParentalControlParams(pctrl_pass, -1, allowed_genres_list);
                    }

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }


    public FunctionResponse SetParentalControlParams(string pctrl_password, int age_limit, int[] allowed_genres){
        return new FunctionResponse(false, "");
    }
}