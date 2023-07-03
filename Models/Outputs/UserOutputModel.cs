public class UserOutputModel{
    public int user_id{get; set;}
    public string full_name {get; set;}
    public string email {get; set;}
    public DateTime createdAt{get; set;}
    public DateTime dob {get; set;}


    public UserOutputModel(int user_id, string full_name, string email, DateTime dob, DateTime cat){
        this.user_id = user_id;
        this.full_name = full_name;
        this.email =email;
        this.dob = dob;
        this.createdAt = cat;
    }
}