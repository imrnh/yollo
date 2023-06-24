class RatingCounterModel{
    public int movie_id {get; set;}
    public float rating_val {get; set;}

    public RatingCounterModel(int mid, float rval){
        this.movie_id = mid;
        this.rating_val = rval;
    }
}