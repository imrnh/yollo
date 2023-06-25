using DotNetEnv;
using Npgsql;
namespace Netflix.Services.user;

public class MovieFilteringService
{
    private readonly string connectionString;
    public MovieFilteringService()
    {
        DotNetEnv.Env.Load();
        this.connectionString = Env.GetString("CONNECTION_STRING");
    }

    public FunctionResponse FilterByGenre(int genre_id)
    {
        List<MovieModel> movies = new List<MovieModel>();
        return new FunctionResponse(false, null);
        
        using (var connection = new NpgsqlConnection(this.connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT * FROM movie_genres", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int Id = Convert.ToInt32(reader["id"]);
                        string Name = reader["name"].ToString();

                        PublisherModel publisher = new PublisherModel(Id, Name);
                        movies.Add(publisher);
                    }
                }
            }
        }
    }
}