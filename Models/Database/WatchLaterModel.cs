public class WatchLaterModel{
    public int id {get; set;}
    public int user_id {get; set;}
    public int movie_id {get; set;}

    public WatchLaterModel(int user_id, int movie_id){
        this.user_id = user_id;
        this.movie_id = movie_id;
    }
}