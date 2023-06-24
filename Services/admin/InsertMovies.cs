using Npgsql;
using DotNetEnv;
using System;

public class ACreateMovies
{
    public static void InsertMovie(string title, string description, DateTime publishedAt, int ageLimit, string bannerUrl, string[] movieFiles, int numberOfEpisodes, bool isSeries)
    {
        DotNetEnv.Env.Load();
        string _connectionString = Env.GetString("CONNECTION_STRING");

        using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            string sql = @"INSERT INTO movie (title, description, published_at, age_limit, banner_url, movie_files, no_of_episodes, isSeries)
                       VALUES (@Title, @Description, @PublishedAt, @AgeLimit, @BannerUrl, @MovieFiles, @NumberOfEpisodes, @IsSeries)";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@PublishedAt", publishedAt);
                command.Parameters.AddWithValue("@AgeLimit", ageLimit);
                command.Parameters.AddWithValue("@BannerUrl", bannerUrl);
                command.Parameters.AddWithValue("@MovieFiles", movieFiles);
                command.Parameters.AddWithValue("@NumberOfEpisodes", numberOfEpisodes);
                command.Parameters.AddWithValue("@IsSeries", isSeries);

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}