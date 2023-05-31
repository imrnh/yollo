public class LoadGenresController
{
    private readonly NetflixDbAccessModel _dbContext;

    public LoadGenresController(NetflixDbAccessModel dbContext)
    {
        _dbContext = dbContext;
    }

    public List<GenereModel> GetAllGenres()
    {
        return _dbContext.ReadGenres().ToList();
    }
}