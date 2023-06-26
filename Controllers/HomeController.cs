using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Netflix.Models;
using Netflix.Services.user;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

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

        return Json(new { all_movies = movies });
    }



    /****

        - GET method

        - for a given slug, this will search for a movie or a series to watch.
        - if found anything, return a MovieModel.
        - if found nothing, return empty array.


    ****/

    [Authorize(Roles = "user")]
    public IActionResult Watch(string slug)
    {
        //fetch movie of that id.
        FunctionResponse response = new ReadMoviesService().SingleMovie(slug);
        if (!response.status)
            return Json(new HttpResponse(401, response.value).toJson());
        return Json(new { movie = response.value });
    }



    /****

        - GET Method.
        - Filter movie based on genre or publisher or age limit.
    
    
    ****/

    [Authorize(Roles = "user")]
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




    /****

        - GET Method.
        - search user to make friend.
        - Searching through email or Full name.
        - Email must match fully.
        - Fullname match ar partial.

    ****/
    
    [Authorize(Roles = "user")]
    public IActionResult SearchFollower(string email, string name)
    {
        if (email == null)
            email = "";
        if (name == null)
            name = "";

        FunctionResponse response = new FriendService().SearchUser(email, name);
        if (!response.status)
        {
            return Json(new HttpResponse(401, response.value).toJson());
        }
        return Json(new { users = response.value });
    }





    /****
        - POST method.
        -a user id will be passed.
        - passed user will be then a follower of the current user.
        - Current user can then check the follower's public contents.
        - The user who is the 2nd user i.e. the person who is being followed will not see anything of the user-1.
    ****/

    [HttpPost]
    [Authorize(Roles = "user")]
    public IActionResult MakeFollower([FromBody] FriendInputModel model, [FromHeader(Name = "Authorization")] string token)
    {
        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());


        int my_id = MyIdFromToken(token);

        //make friend here
        FunctionResponse response = new FriendService().MakeFriend(my_id, model.Id);

        return Json(new HttpResponse(200, response.value));
    }




    /****
    
        - GET type.
        - Must be signed in.
        - Retrive `my_id` from the token.
        - Get all the user_2 from friends table for user_1 = my_id.
    
    ****/

    [Authorize(Roles = "user")]
    public IActionResult LoadFriends([FromHeader(Name = "Authorization")] string token)
    {
        int my_id = MyIdFromToken(token);

        FunctionResponse response = new FriendService().LoadFriend(my_id);
        return Json(new { friends = response.value });
    }



    /***

        ERROR Response model.

    ***/

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }




    /****
    
        - Utility function.

        Token collection method as following:
            - Read the header and get the value of the key `Authorization`.
            

        When the token is passed to this function, the token normally looks like this:
            `Bearer<space> Actual token value`. Therefore, total 7 letter (`Bearer<space>`) should be removed from the prefix.

        Then the user email is fetched from the token. Because the user_email is passed for the ClaimType.Names when creating the token.
        Another service function convert the email into id from databse.

        This function then return the id. If the token is invalid, it will return -1 as there will be no id to fetch. 
    
    ****/

    private int MyIdFromToken(string token)
    {
        string my_email = new AuthenticationService().GetEmailFromToken(token.Remove(0, 7)); //removing the prefix "Bearer " from the passed Authorization token.

        FunctionResponse uid_resp = new AuthenticationService().GetUserIdFromEmail(my_email);
        if (!uid_resp.status)
            return -1;
        return uid_resp.value;
    }
}
