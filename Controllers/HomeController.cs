using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Netflix.Models;
using Netflix.Services.user;

namespace Netflix.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ReadMoviesService readMoviesService = new ReadMoviesService();
        List<MovieModel> movies = readMoviesService.ReadMovies(6);
        
        
        return Json(new {all_movies=movies});
    }
    
    public IActionResult Movie(int movie)
    {
        //fetch movie of that id.
        return Json(new {data = movie});
    }

    public IActionResult Movies(int genre, int publisher)
    {
        //fetch all the movies of that genre.
        if (genre != 0 && publisher != 0)
            return Json(new { genre = genre, publisher = publisher });
        else if (genre != 0)
            return Json(new { movies = new MovieFilteringService().FilterByGenre(genre) });
        else if (publisher != 0)
            return Json(new { movies = new MovieFilteringService().FilterByPublisher(publisher) });

        return Json(new HttpResponse(401, "Invalid response").toJson());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}