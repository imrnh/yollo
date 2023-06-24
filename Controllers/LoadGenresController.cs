/*
    - Do not delete 
    this will load all the genres for the navigation bar.
*/

public class LoadGenresController
{
    private readonly NetflixDbAccessModel _dbContext;

    public LoadGenresController(NetflixDbAccessModel dbContext)
    {
        _dbContext = dbContext;
    }

    public List<GenreModel> GetAllGenres()
    {
        return _dbContext.ReadGenres().ToList();
    }
}