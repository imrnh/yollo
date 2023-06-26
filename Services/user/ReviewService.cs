using DotNetEnv;
using Npgsql;



namespace Netflix.Services.user;
public class ReviewService
{

    private string connectionString;
    public ReviewService()
    {
        DotNetEnv.Env.Load();
        this.connectionString = Env.GetString("CONNECTION_STRING");
    }

    public FunctionResponse AddReview(int userId, int movieId, string reviewText, int rating)
    {
        try
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Insert the review data into the "review" table using parameterized query
                string insertQuery = "INSERT INTO review (user_id, movie_id, review, rating) " +
                                     "VALUES (@userId, @movieId, @review, @rating)";
                using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);
                    command.Parameters.AddWithValue("movieId", movieId);
                    command.Parameters.AddWithValue("review", reviewText);
                    command.Parameters.AddWithValue("rating", rating);
                    command.ExecuteNonQuery();
                }

                return new FunctionResponse(true, "Review Added Successfully");
            }
        }
        catch (Exception e)
        {
            return new FunctionResponse(false, e.Message);
        }
    }




    /******
    
        - Get all the reviews for a given movie id.
    
    ******/

    public List<ReviewModel> GetReviews(int movieId)
    {
        List<ReviewModel> reviews = new List<ReviewModel>();
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            // Retrieve all reviews from the "review" table
            string selectQuery = "SELECT id, user_id, movie_id, review, rating FROM review WHERE movie_id=@movieId";
            using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("movieId", movieId);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {


                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        int userId = reader.GetInt32(1);
                        int _movieId = reader.GetInt32(2);
                        string reviewText = reader.GetString(3);
                        int rating = reader.GetInt32(4);

                        ReviewModel review = new ReviewModel(userId, _movieId, reviewText, rating);
                        reviews.Add(review);
                    }

                    return reviews;
                }
            }
        }
    }


    public float CalculateAverageRating(int movieId)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            // Retrieve all ratings for the given movie from the "review" table
            string selectQuery = "SELECT rating FROM review WHERE movie_id = @movieId";
            using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("movieId", movieId);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    int totalRatings = 0;
                    int sumRatings = 0;

                    
                    while (reader.Read())
                    {
                        int rating = reader.GetInt32(0);
                        sumRatings += rating;
                        totalRatings++;
                    }

                    if (totalRatings > 0)
                    {
                        float averageRating = (float)sumRatings / totalRatings;
                        return averageRating;
                    }
                    else
                    {
                        return 0.0f;
                    }
                }
            }
        }
    }
}