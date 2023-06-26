namespace Netflix.Models.Inputs.users;

public class ParentalControlInputModel
{
    public bool Activate { get; set; }
    public string Password { get; set; }
    public int Agelimit { get; set; }

    public int[] Genres { get; set; }
}