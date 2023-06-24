public class PublisherModel{
    public int id {get; set;}
    public string name {get; set;}

    public PublisherModel(int id, string name){
        this.name = name;
        this.id = id;
    }
}