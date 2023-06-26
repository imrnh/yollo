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
}