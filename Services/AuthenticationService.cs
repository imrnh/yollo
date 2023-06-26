using System.Security.Claims;
using System.Security.Cryptography;
using DotNetEnv;
using Npgsql;

public class AuthenticationService
{
    private readonly string connectionString;

    private dynamic salt;
    public AuthenticationService()
    {
        DotNetEnv.Env.Load();
        this.connectionString = Env.GetString("CONNECTION_STRING");
        this.salt = RandomNumberGenerator.GetBytes(64);
    }

    public FunctionResponse CrateUser(string email, string password, string fname, DateTime dob)
    {


        using (var connection = new NpgsqlConnection(this.connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                command.CommandText = "INSERT INTO users (email, password, full_name, dob) VALUES (@email, @password, @fname, @dob)";

                string hashedPassword = PasswordHasher.HashPassword(password);


                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("password", password); //perform hash later. current hash function is not working properly.
                command.Parameters.AddWithValue("fname", fname);
                command.Parameters.AddWithValue("dob", dob);

                try
                {
                    command.ExecuteNonQuery();
                    return new FunctionResponse(true, "New user created");
                }
                catch (PostgresException pgexp)
                {
                    return new FunctionResponse(false, pgexp.MessageText);
                }
            }
            connection.Close();
        }
        return new FunctionResponse(false, null);
    }




    public FunctionResponse LoginUser(string email, string user_input_password)
    {


        using (var connection = new NpgsqlConnection(this.connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                command.CommandText = "SELECT * FROM users where email=@email";
                command.Parameters.AddWithValue("email", email);

                try
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string hashed_password = Convert.ToString(reader["password"]);
                        bool isAdmin = Convert.ToBoolean(reader["isAdmin"]);

                        //perform hashing later.

                        bool hashCheckResult = hashed_password == user_input_password; // PasswordHasher.VerifyPassword(user_input_password, hashed_password);

                        if (!hashCheckResult)
                            return new FunctionResponse(false, "Invalid password");

                        return new FunctionResponse(true, isAdmin);
                    }
                }
                catch (Exception e)
                {
                    return new FunctionResponse(false, e.Message);
                }

            }
            connection.Close();
        }
        return new FunctionResponse(false, null);
    }

    public string GetEmailFromToken(string token)
    {
        try
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var usernameClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            if (usernameClaim != null)
            {
                return usernameClaim.Value;
            }
        }
        catch (Exception e)
        {
            return e.Message;
        }

        // Username claim not found in the token
        return null;
    }


    public FunctionResponse GetUserIdFromEmail(string email)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT id FROM users WHERE email = @Email";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("Email", email);

                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    int userId = Convert.ToInt32(result);
                    return new FunctionResponse(true, userId);
                }
                else
                {
                    return new FunctionResponse(false, null);
                }
            }
        }
        return new FunctionResponse(false, null);
    }
}