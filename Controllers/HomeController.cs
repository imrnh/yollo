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
    



    /*

        - GET method

        - for a given slug, this will search for a movie or a series to watch.
        - if found anything, return a MovieModel.
        - if found nothing, return empty array.


    */


    public IActionResult Watch(string slug)
    {
        //fetch movie of that id.
        FunctionResponse response = new ReadMoviesService().SingleMovie(slug);
        if(!response.status)
            return Json(new  HttpResponse(401, response.value).toJson());
        return Json(new {movie = response.value});
    }



    /*

        - GET Method.
        - Filter movie based on genre or publisher or age limit.
    
    
    */
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


    /*
        - search user to make friend.
    */

    public IActionResult SearchFollower(string email, string name){
        if (email == null)
            email = "";
        if (name == null)
            name = "";
            
        FunctionResponse response = new FriendService().SearchUser(email, name);
        if(!response.status){
            return Json(new HttpResponse(401, response.value).toJson());
        }
        return Json(new {users=response.value});
    }


    /*
        - POST method.
        -a user id will be passed.
        - passed user will be then a follower of the current user.
        - Current user can then check the follower's public contents.
        - The user who is the 2nd user i.e. the person who is being followed will not see anything of the user-1.
    */

    [HttpPost]
    public IActionResult MakeFollower([FromBody] FriendInputModel model)
    {
        if(!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());

        //make friend here
       
        return Json(new {users=""});
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}