namespace Netflix.Models.Inputs;

public class SignupInpModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public DateTime Dob { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool ParentalControlActive { get; set; } = false;
    public DateTime CreatedAt { get; set; }
}


public class SignInInpModel
{
    public string Email { get; set; }
    public string Password { get; set; }

}