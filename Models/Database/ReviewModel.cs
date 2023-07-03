public class ReviewModel{
    public int id {get; set;}
    public int user_id {get; set;}
    public int movie_id {get; set;}
    public string review{get; set;}
    public int rating {get; set;}
    public string user_name {get; set;}

    public ReviewModel( int user_id, int movie_id, string review, int rating){
        this.user_id = user_id;
        this.movie_id = movie_id;
        this.review = review;
        this.rating = rating;
    }
}