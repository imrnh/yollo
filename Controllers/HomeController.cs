﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Netflix.Models;
using Netflix.Services.user;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Netflix.Models.Inputs.users;

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
        List<MovieModel> movies = new ReadMoviesService().ReadMovies(6);
        return Json(new { all_movies = movies });
    }



    /****
     * 
     * GET method.
     * Read all the genre names available.
     * 
     * ****/

    [Authorize(Roles = "user")]
    public IActionResult AllGenres()
    {
        List<GenreModel> genres = new ReadMoviesService().ReadGenres();
        return Json(new { genres = genres });
    }

    /****
     * 
     * POST method.
     * Read all the genre names available.
     * 
     * ****/

    [Authorize(Roles = "user")]
    public IActionResult GenreIdToName([FromBody] GenreIdInputModel gmodel)
    {
        FunctionResponse response = new ReadMoviesService().GenreNameFromId(gmodel.Id);
        return Json(new { genre_name = response.value });
    }


    /****

    - POST method

    - take user id from my token.
    - pctrl param is boolean.


    ****/

    [Authorize(Roles = "user")]
    public IActionResult ParentalControl([FromHeader(Name = "Authorization")] string token, [FromBody] ParentalControlInputModel pctrl_model)
    {
        if (!ModelState.IsValid)
            return Json(new { error = "Model state is not valid" });

        int my_id = MyIdFromToken(token);
        FunctionResponse message = new ParentalControlService().ToggleParentalControl(my_id, pctrl_model.Activate, pctrl_model.Password, pctrl_model.Agelimit, pctrl_model.Genres);
        return Json(new { message = message.value });
    }


    /**
    - GET method.
    - Check if parental control active or not.

**/

    [Authorize(Roles = "user")]
    public IActionResult PctrlActiveStatus([FromHeader(Name = "Authorization")] string token)
    {
        int my_id = MyIdFromToken(token);
        return Json(new { pctrl_status = new ParentalControlService().CheckParentalControlActive(my_id) });
    }


    /****

        - GET method

        - for a given slug, this will search for a movie or a series to watch.
        - if found anything, return a MovieModel.
        - if found nothing, return empty array.


    ****/

    [Authorize(Roles = "user")]
    public IActionResult Watch([FromHeader(Name = "Authorization")] string token, string slug)
    {
        int my_id = MyIdFromToken(token);

        //fetch movie of that id.
        FunctionResponse response = new ReadMoviesService().SingleMovie(slug);
        if (!response.status)
            return Json(new HttpResponse(401, response.value).toJson());

        FunctionResponse filter_response = new ParentalControlService().FilterWithParentalControl(response.value, my_id);
        return Json(new { movie = filter_response.value });
    }



    /****

        - GET method

        - for a given slug, this will search for a movie or a series to watch.
        - if found anything, return a MovieModel.
        - if found nothing, return empty array.


    ****/

    [Authorize(Roles = "user")]
    public IActionResult SearchMovies([FromHeader(Name = "Authorization")] string token, string q)
    {
        int my_id = MyIdFromToken(token);

        //fetch movie of that id.
        List<MovieModel> movies = new ReadMoviesService().SearchMovies(q);
        FunctionResponse filtered_response = new ParentalControlService().FilterWithParentalControl(movies, my_id);

        List<MovieModel> allowed_movies = filtered_response.value;

        return Json(new { movie = allowed_movies });
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
        - GET method.
        - Take no extra param. Just read the header for auth token.
        - Generate id from token.
        - Pass to a function of SavedMovieServies to get movie ids.
        - Pass the ids to another function one by one to get movies.
    
    ****/

    [Authorize(Roles = "user")]
    public IActionResult WatchHistory([FromHeader(Name = "Authorization")] string token)
    {
        int my_id = MyIdFromToken(token);

        List<int> movie_ids = new List<int>();
        List<MovieModel> movies = new List<MovieModel>();

        FunctionResponse load_resp = new SavedMoviesService().LoadWatchHistory(my_id);
        if (!load_resp.status)
            return Json(new HttpResponse(401, load_resp.value));

        movie_ids = load_resp.value; //if no errror, get all the movie ids.

        foreach (var mvid in movie_ids)
        {
            FunctionResponse mv_resp = new ReadMoviesService().SingleMovie("", mvid);
            if (!mv_resp.status)
                return Json(new HttpResponse(401, mv_resp.value).toJson());

            movies.Add(mv_resp.value);
        }


        return Json(new { movies = movies, me = my_id });
    }



    /****
       - GET method.
       - Take no extra param. Just read the header for auth token.
       - Generate id from token.
       - Pass to a function of SavedMovieServies to get movie ids.
       - Pass the ids to another function one by one to get movies.

   ****/

    [Authorize(Roles = "user")]
    public IActionResult WatchLater([FromHeader(Name = "Authorization")] string token)
    {
        int my_id = MyIdFromToken(token);

        List<int> movie_ids = new List<int>();
        List<MovieModel> movies = new List<MovieModel>();

        FunctionResponse load_resp = new SavedMoviesService().LoadWatchLater(my_id);
        if (!load_resp.status)
            return Json(new HttpResponse(401, load_resp.value));

        movie_ids = load_resp.value; //if no errror, get all the movie ids.

        foreach (var mvid in movie_ids)
        {
            FunctionResponse mv_resp = new ReadMoviesService().SingleMovie("", mvid);
            if (!mv_resp.status)
                return Json(new HttpResponse(401, mv_resp.value).toJson());

            movies.Add(mv_resp.value);
        }


        return Json(new { movies = movies, me = my_id });
    }



    /****

        - POST method.
        - Read my_id from authorization token.
        - then read value from query param.
    
    ****/
    [Authorize(Roles = "user")]
    public IActionResult TriggerWhl([FromHeader(Name = "Authorization")] string token, string flag, bool b)
    {
        int int_flag = flag == "wh" ? 0 : (flag == "wl" ? 1 : -1); // if get -1, function does nothing.

        int my_id = MyIdFromToken(token);

        FunctionResponse response = new SavedMoviesService().Trigger_WHL_Public(my_id, b, int_flag);

        return Json(new { response.value });
    }




    /****

        - POST method.
        - Read my_id from authorization token.
        - then read movie id from body.
        - Pass to function
    
    ****/

    [Authorize(Roles = "user")]
    public IActionResult AddMovieToWatchHistory([FromHeader(Name = "Authorization")] string token, [FromBody] MovieIdInpModel model)
    {
        int my_id = MyIdFromToken(token);

        FunctionResponse response = new SavedMoviesService().SavedMovieToWatchHistory(my_id, model.MovieId);

        return Json(new { response.value });
    }



    /****

        - POST method.
        - Read my_id from authorization token.
        - then read movie id from body.
        - Pass to function
    
    ****/

    [Authorize(Roles = "user")]
    public IActionResult AddMovieToWatchLater([FromHeader(Name = "Authorization")] string token, [FromBody] MovieIdInpModel model)
    {
        int my_id = MyIdFromToken(token);

        FunctionResponse response = new SavedMoviesService().SavedMovieToWatchLater(my_id, model.MovieId);

        return Json(new { response.value });
    }




    /****

    - POST method.
    - Read my_id from authorization token.
    - then read movie id from body.
    - Pass to function to delete

****/

    [Authorize(Roles = "user")]
    public IActionResult DeleteMovieFromWatchHistory([FromHeader(Name = "Authorization")] string token, [FromBody] MovieIdInpModel model)
    {
        int my_id = MyIdFromToken(token);

        FunctionResponse response = new SavedMoviesService().RemoveFromWatchHistory(my_id, model.MovieId);

        return Json(new { response.value });
    }



    /****

    - POST method.
    - Read my_id from authorization token.
    - then read movie id from body.
    - Pass to function to delete from watch later

****/

    [Authorize(Roles = "user")]
    public IActionResult DeleteMovieFromWatchLater([FromHeader(Name = "Authorization")] string token, [FromBody] MovieIdInpModel model)
    {
        int my_id = MyIdFromToken(token);

        FunctionResponse response = new SavedMoviesService().RemoveFromWatchLater(my_id, model.MovieId);

        return Json(new { response.value });
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
         - POST method.
         - a user id will be passed.
         - passed user will be removed from follower of the current user.
         - Current user can no longer check the follower's public contents.
     ****/

    [HttpPost]
    [Authorize(Roles = "user")]
    public IActionResult RemoveFollower([FromBody] FriendInputModel model, [FromHeader(Name = "Authorization")] string token)
    {
        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());

        int my_id = MyIdFromToken(token);

        //make friend here
        FunctionResponse response = new FriendService().RemoveFriend(my_id, model.Id);

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




    /****
        - POST method.
        - a user id will be passed.

        -- Load watch history.

            - The function will check the if wh_history of the user publicly visible.
            - If visible, load them.

        -- Then load watch later.
            - Check wl_public for that user.
            - If visible, load them.
        - 
     ****/

    [HttpPost]
    [Authorize(Roles = "user")]
    public IActionResult FriendWatchHistoryNWatchLater([FromBody] FriendInputModel model, [FromHeader(Name = "Authorization")] string token)
    {
        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());


        //make friend here
        bool[] whl_state = new SavedMoviesService().Check_WHL_Public(model.Id); // wh -> 0th index, wl -> 1st index

        Dictionary<string, object> whl_obj = new Dictionary<string, object>();

        //initialiing with empty array
        whl_obj["watch_history"] = new List<MovieModel>();
        whl_obj["watch_later"] = new List<MovieModel>();


        if (whl_state[0])
        { //if watch history visible.
            List<int> movie_ids = new List<int>();
            List<MovieModel> movies = new List<MovieModel>();
            FunctionResponse load_resp = new SavedMoviesService().LoadWatchHistory(model.Id);
            if (!load_resp.status)
                return Json(new HttpResponse(401, load_resp.value));

            movie_ids = load_resp.value; //if no errror, get all the movie ids.

            foreach (var mvid in movie_ids)
            {
                FunctionResponse mv_resp = new ReadMoviesService().SingleMovie("", mvid);
                if (!mv_resp.status)
                    return Json(new HttpResponse(401, mv_resp.value).toJson());

                movies.Add(mv_resp.value);
            }
            whl_obj["watch_history"] = movies;
        }

        if (whl_state[1])
        {// if watch later visible.


            List<int> movie_ids = new List<int>();
            List<MovieModel> movies = new List<MovieModel>();

            FunctionResponse load_resp = new SavedMoviesService().LoadWatchLater(model.Id);
            if (!load_resp.status)
                return Json(new HttpResponse(401, load_resp.value));

            movie_ids = load_resp.value; //if no errror, get all the movie ids.

            foreach (var mvid in movie_ids)
            {
                FunctionResponse mv_resp = new ReadMoviesService().SingleMovie("", mvid);
                if (!mv_resp.status)
                    return Json(new HttpResponse(401, mv_resp.value).toJson());

                movies.Add(mv_resp.value);
            }

            whl_obj["watch_later"] = movies;
        }


        return Json(new { movies = whl_obj });
    }






    /**
        - POST method.
        - post a review
    
    **/

    [HttpPost]
    [Authorize(Roles = "user")]
    public IActionResult PostReview([FromBody] ReviewInputModel model, [FromHeader(Name = "Authorization")] string token)
    {
        int my_id = MyIdFromToken(token);

        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Invalid input with model").toJson());

        FunctionResponse response = new ReviewService().AddReview(my_id, model.Mid, model.Review, model.Rating);

        if (!response.status)
            return Json(new HttpResponse(401, response.value).toJson());

        return Json(new HttpResponse(200, response.value).toJson());
    }


    /**
        - GET method.
        - Get reviews of the movie.
    
    **/

    [Authorize(Roles = "user")]
    public IActionResult GetAllReviews(int mvi)
    {
        return Json(new { reviews = new ReviewService().GetReviews(mvi) });
    }



    /**
        - GET method.
        - Get average rating count for movie.
    
    **/

    [Authorize(Roles = "user")]
    public IActionResult GetAvgRating(int mvi)
    {
        return Json(new { ratings = new ReviewService().CalculateAverageRating(mvi) });
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
