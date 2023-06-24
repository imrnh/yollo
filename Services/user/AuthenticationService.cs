namespace Netflix.Services.user;
using DotNetEnv;

public class AuthenticationService
{
    private readonly string connectionString;
    public AuthenticationService()
    {
        DotNetEnv.Env.Load();
        this.connectionString = Env.GetString("CONNECTION_STRING");
    }
}