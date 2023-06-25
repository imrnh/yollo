using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Netflix.Models;
using Netflix.Services.user;
using System.Collections.Generic;
using System.Linq;

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
    
    public IActionResult Watch(string slug)
    {
        //fetch movie of that id.
        FunctionResponse response = new ReadMoviesService().SingleMovie(slug);
        if(!response.status)
            return Json(new  HttpResponse(401, response.value).toJson());
        return Json(new {movie = response.value});
    }

    public IActionResult Movies(int genre, int publisher, int age_limit)
    {
        List<MovieModel> filtered = new List<MovieModel>();
        
        //fetch all the movies of that genre.
        if (genre != 0 && publisher != 0)
        {
            List<MovieModel> filterdByGenre = new List<MovieModel>();
            List<MovieModel> filterdByPublishers = new List<MovieModel>();

            FunctionResponse genre_resp = new MovieFilteringService().FilterByGenre(genre);
            if (!genre_resp.status) //in-case of an error
                return Json(new HttpResponse(401, genre_resp.value).toJson());
            filterdByGenre = genre_resp.value;
            
            FunctionResponse publisher_resp = new MovieFilteringService().FilterByPublisher(publisher);
            if (!publisher_resp.status) //in-case of an error
                return Json(new HttpResponse(401, publisher_resp.value).toJson());
            filterdByPublishers = publisher_resp.value;

            filtered = MovieFilteringService.GetCommonMovies(filterdByGenre, filterdByPublishers).value;

            return Json(new { movies = filtered });
        }
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