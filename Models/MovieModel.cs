public class MovieModel{

    public MovieModel(int id, string name, string description, int publisher_id, int publishing_year, string banner_url){
        this.Id = id;
        this.Name = name;
        this.description = description;
        this.publisher_id = publisher_id;
        this.publishing_year = publishing_year;
        this.banner_url = banner_url;
    }
    public int Id {get; set;}
    public string Name {get; set;}
    public string description{get; set;}
    
    public int publisher_id {get; set;}

    public int publishing_year {get; set;}

    public List<int> genre_ids {get; set;}

    public string banner_url {get; set;}
}