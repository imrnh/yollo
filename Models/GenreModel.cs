public class GenereModel{
    
    public GenereModel(){

    }

    public GenereModel(int id, string name){
        this.Id = id;
        this.Name = name;
    }
    public int Id {get; set; }
    public string Name {get; set; }
}