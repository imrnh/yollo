public class ShowUploadModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int[] Genres {get; set;}
    public int[] Publishers {get; set;}

    public DateTime PublishedAt { get; set; }
    public int AgeLimit { get; set; }
    public IFormFile Banner { get; set; }
    public List<IFormFile> Movies { get; set; }
    public int NoOfEpisodes { get; set; }
    public bool IsSeries { get; set; }

}