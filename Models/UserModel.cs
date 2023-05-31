public class UserModel{
    public UserModel(string email, string password, string full_name, string dob){
        this.email = email;
        this.password = password;
        this.full_name = full_name;
        this.dob = dob;
    }

    public int id {get; set;}
    public string email {get; set;}
    public string password {get; set;}
    public string full_name {get; set;}
    public string dob {get; set;}
}