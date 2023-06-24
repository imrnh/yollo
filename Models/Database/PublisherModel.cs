public class PublisherModel{
    public int id {get; set;}
    public string name {get; set;}
    public int founding_year {get; set;}

    public PublisherModel(string name, int founding_year){
        this.name = name;
        this.founding_year = founding_year;
    }
}