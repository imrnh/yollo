public class MovieModel
{
    public int id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public List<int> genres { get; set; }

    public List<int> publishers { get; set; }

    public DateTime published_at { get; set; }

    public int age_limit { get; set; }

    public string banner_url { get; set; }

    public List<string> movie_files { get; set; }

    public int no_of_episods { get; set; }

    public bool isSeries { get; set; }

    public string slug {get; set;}


    //parameterized constructor.

    public MovieModel(int id, string name, string description, List<int> genres, List<int> publishers, DateTime published_at, int age_limit, string banner_url, List<string> movie_files, int no_of_episods, bool isSeries)
    {
        this.id = id;
        this.title = name;
        this.description = description;
        this.genres = genres;
        this.publishers = publishers;
        this.published_at = published_at;
        this.age_limit = age_limit;
        this.banner_url = banner_url;
        this.movie_files = movie_files;
        this.no_of_episods = no_of_episods;
    }


    public MovieModel(int id, string name, string description, DateTime published_at, int age_limit, string banner_url, List<string> movie_files, int no_of_episods, bool isSeries)
    {
        this.id = id;
        this.title = name;
        this.description = description;
        this.published_at = published_at;
        this.age_limit = age_limit;
        this.banner_url = banner_url;
        this.movie_files = movie_files;
        this.no_of_episods = no_of_episods;
    }



    //default constructor.
    public MovieModel()
    {

    }

}