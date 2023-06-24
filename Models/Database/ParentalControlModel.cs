public class ParentalControlModel{
    public int user_id  {get; set;}
    public string pctr_password {get; set;} //password to on or off parental control
    public int age_limit  {get; set;} //maximum age of children
    public int[] allowed_genres {get; set;}
    public int[] allowed_movies {get; set;}


    public ParentalControlModel(int uid, string pass, int agelimit, int[]  genres, int[] movies){
        this.user_id = uid;
        this.pctr_password = pass;
        this.age_limit = agelimit;
        this.allowed_genres = genres;
        this.allowed_movies = movies;
    }

}