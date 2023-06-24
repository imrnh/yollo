namespace Netflix.Models.Inputs;

public class SignupInpModel
{
    public string email { get; set; }
    public string password { get; set; }
    public string fullname { get; set; }
    public DateTime dob { get; set; }
}


public class SignInInpModel
{
    public string email { get; set; }
    public string password { get; set; }
    public string fullname { get; set; }
    public DateTime dob { get; set; }
}