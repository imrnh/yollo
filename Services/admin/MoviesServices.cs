using Npgsql;
using DotNetEnv;
using System;

public class AMovieServices
{
    private string _connectionString;
    public AMovieServices()
    {
        DotNetEnv.Env.Load();
        this._connectionString = Env.GetString("CONNECTION_STRING");
    }


    private FunctionResponse GetMovieId(string title)
    {
        using (var connection = new NpgsqlConnection(this._connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT id FROM movie WHERE title = @title", connection))
            {
                command.Parameters.AddWithValue("title", title);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int movieId = Convert.ToInt32(reader["id"]);
                        return new FunctionResponse(true, movieId);
                    }
                }

            }
        }
        return new FunctionResponse(false, -1);
    }


    private FunctionResponse ConnectMovieAndGenres(int[] genres, int movieId)
    {
        foreach (var gnr in genres)
        {
            using (var connection = new NpgsqlConnection(this._connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("INSERT INTO movie_genres (movie_id, genre_id) VALUES (@movieId, @genreId)", connection))
                {
                    command.Parameters.AddWithValue("movieId", movieId);
                    command.Parameters.AddWithValue("genreId", gnr);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (PostgresException pgexp)
                    {
                        return new FunctionResponse(false, pgexp.Message);
                    }
                }
            }
        }
        return new FunctionResponse(true, null);
    }

    private FunctionResponse ConnectMovieAndPublishers(int[] publishers, int movieId)
    {
        foreach (var pbr in publishers)
        {
            using (var connection = new NpgsqlConnection(this._connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("INSERT INTO movie_publishers (movie_id, publisher_id) VALUES (@movieId, @pubId)", connection))
                {
                    command.Parameters.AddWithValue("movieId", movieId);
                    command.Parameters.AddWithValue("pubId", pbr);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                   catch (PostgresException pgexp)
                    {
                        return new FunctionResponse(false, pgexp.Message);
                    }
                }
            }
        }
        return new FunctionResponse(true, null);
    }


    public FunctionResponse InsertMovie(string title, string description, int[] genres, int[] publishers, DateTime publishedAt, int ageLimit, string bannerUrl, string[] movieFiles, int numberOfEpisodes, bool isSeries)
    {


        using (NpgsqlConnection connection = new NpgsqlConnection(this._connectionString))
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

                try
                {

                    command.ExecuteNonQuery();

                    FunctionResponse response = GetMovieId(title);

                    if (!response.status)
                    {
                        throw new Exception("Error reading id from database of the inserted movie");
                    }
                    int insertedId = response.value;

                    FunctionResponse gcon_res = ConnectMovieAndGenres(genres, insertedId);

                    if (!gcon_res.status)
                    {
                        throw new Exception(gcon_res.value);
                    }

                    FunctionResponse pucon_res = ConnectMovieAndPublishers(publishers, insertedId);
                    if (!pucon_res.status)
                    {
                        throw new Exception(pucon_res.value);
                    }

                    return new FunctionResponse(true, "Movie inserted successfully");
                }
                catch (PostgresException pgexp)
                {
                    return new FunctionResponse(false, pgexp.MessageText);
                }
                catch (Exception e)
                {
                    return new FunctionResponse(false, e.Message);
                }
            }

            connection.Close();
        }
    }
}