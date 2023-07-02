using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


using Netflix.Models;

namespace Netflix.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    public IActionResult Dashboard()
    {
        return Json(new { controller_name = "Admin" });
    }


    [HttpPost]
    public IActionResult VideoUpload()
    {

        try
        {
            Random random = new Random();
            string randomString = "";

            for (int i = 0; i < 2; i++)
            {
                int randomNumber = random.Next(100000, 1000000000);
                randomString += randomNumber.ToString();
            }

            List<string> movie_files_path = new List<string>();
            string banner_path = "";

            string title = Request.Form["title"];
            string description = Request.Form["desc"];
            int.TryParse(Request.Form["age_limit"], out int ageLimit);
            DateTime.TryParse(Request.Form["published_at"], out DateTime publishedAt);
            int.TryParse(Request.Form["no_eps"], out int noOfEpisodes);
            bool.TryParse(Request.Form["isseries"], out bool isSeries);


            IFormFile imageFile = Request.Form.Files["banner"];
            IFormFile videoFile = Request.Form.Files["video"];

            string genreStrings = Request.Form["genres"];
            string[] genre_strings_sep = genreStrings.Split(", ");
            List<int> genres = new List<int>();
            foreach (string gsingle in genre_strings_sep)
            {
                genres.Add(int.Parse(gsingle));
            }


            string publishersString = Request.Form["publishers"];
            string[] publishers_strings_sep = publishersString.Split(", ");
            List<int> publishers = new List<int>();
            foreach (string psi in publishers_strings_sep)
            {
                publishers.Add(int.Parse(psi));
            }

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string fileName = randomString + title + Path.GetFileName(imageFile.FileName);
                    banner_path = fileName;
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(fileStream);
                    }

                }
                else
                {
                    return Json(new { success = false, message = "No Image file was uploaded" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading Image: " + ex.Message });
            }


            try
            {
                if (videoFile != null && videoFile.Length > 0)
                {
                    string fileName = randomString + title + Path.GetFileName(videoFile.FileName);
                    movie_files_path.Add(fileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        videoFile.CopyTo(fileStream);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "No video file was uploaded" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading video: " + ex.Message });
            }



            FunctionResponse response = new AMovieServices().InsertMovie(title, description, genres.ToArray(), publishers.ToArray(), publishedAt, ageLimit, banner_path, movie_files_path.ToArray(), noOfEpisodes, isSeries);
            if (!response.status)
                return Json(new HttpResponse(401, response.value).toJson());

            return Json(new HttpResponse(200, "New movie named '" + title + "' added succesfully").toJson());
        }
        catch (Exception e)
        {
            return Json(new HttpResponse(400, e.Message).toJson());
        }
    }



    /*****
    
        - All genres
            -> Read all genre names and id from genre table and return the result as a list of GenreModels

        - UpdateGenre
            -> Take id and new name as query param and update the genre name of the give id. If done correctly, return true. else return false. No error message is sent.

        - DeleteGenre
            -> take id as query param and delete from genre table. Return boolean response.
    
    ******/


    [HttpPost]
    [Authorize(Roles = "admin")]
    public IActionResult AddGenre([FromBody] GenreInputModel model)
    {
        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());

        FunctionResponse response = new AGenreServices().InsertGenre(model.Name);
        if (!response.status)
            return Json(new HttpResponse(401, response.value).toJson());

        return Json(new HttpResponse(200, "Genre '" + model.Name + "' added succesfully").toJson());
    }



    [Authorize(Roles = "admin")]
    public IActionResult AllGenres()
    {
        List<GenreModel> genres = new AGenreServices().GetAllGenres();
        return Json(new { all_genres = genres });
    }




    [Authorize(Roles = "admin")]
    public IActionResult UpdateGenre(int genre_id, string genre_name)
    {
        bool response = new AGenreServices().UpdateGenreName(genre_id, genre_name);
        return Json(new { success = response });
    }



    [Authorize(Roles = "admin")]
    public IActionResult DeleteGenre(int genre_id)
    {
        bool response = new AGenreServices().DeleteGenre(genre_id);
        return Json(new { success = response });
    }




    /*****
    
        - AllPublishers
            -> Read all publisher's name and id from publishers table and return the result as a list of PublisherModels

        - UpdatePublisher
            -> Take id and new name as query param and update the publisher name of the give id. If done correctly, return true. else return false. No error message is sent.

        - DeletePublisher
            -> take id as query param and delete from publihser table. Return boolean response.
    
    ******/


    [HttpPost]
    [Authorize(Roles = "admin")]
    public IActionResult AddPublisher([FromBody] PublisherInputModel model)
    {
        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());

        FunctionResponse response = new APublisherServices().InsertPublisher(model.Name);
        if (!response.status)
            return Json(new HttpResponse(401, response.value).toJson());

        return Json(new HttpResponse(200, "Publisher '" + model.Name + "' added succesfully").toJson());
    }


    public IActionResult AllPublishers()
    {
        List<PublisherModel> publishers = new APublisherServices().GetAllPublishers();
        return Json(new { all_publishers = publishers });
    }


    [Authorize(Roles = "admin")]
    public IActionResult UpdatePublisher(int pub_id, string pub_name)
    {
        bool response = new APublisherServices().UpdatePublisherName(pub_id, pub_name);
        return Json(new { success = response });
    }



    [Authorize(Roles = "admin")]
    public IActionResult DeletePublisher(int pub_id)
    {
        bool response = new APublisherServices().DeletePublisher(pub_id);
        return Json(new { success = response });
    }






    /*****
    
        - AddMoviesOrSeries
            -> Add a new movie or tv-show to the movies table nd return HttpReponse as json.

        - MovieGenres
            -> Post method
            -> Take id and new name as post body input and return the genres of a movie.
    
    ******/



    [HttpPost]
    [Authorize(Roles = "admin")]
    public IActionResult AddMovieOrSeries([FromBody] MovieInputModel model)
    {


        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());

        FunctionResponse response = new AMovieServices().InsertMovie(
            model.Title,
            model.Description,
            model.Genres,
            model.Publishers,
            model.PublishedAt,
            model.AgeLimit,
            model.BannerUrl,
            model.MovieFiles,
            model.NoOfEpisodes,
            model.IsSeries
        );
        if (!response.status)
            return Json(new HttpResponse(401, response.value).toJson());

        return Json(new HttpResponse(200, "New movie named '" + model.Title + "' added succesfully").toJson());
    }



    [HttpPost]
    [Authorize(Roles = "admin")]
    public IActionResult MovieGenres([FromBody] MovieIdInpModel model)
    {
        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());

        FunctionResponse response = new AMovieServices().GetGenresByMovieId(model.MovieId);
        if (!response.status)
            return Json(new HttpResponse(401, response.value).toJson());

        return Json(new { genres = response.value }); //response contain the value.
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public IActionResult MoviePublishers([FromBody] MovieIdInpModel model)
    {
        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());

        FunctionResponse response = new AMovieServices().GetPublishersByMovieId(model.MovieId);
        if (!response.status)
            return Json(new HttpResponse(401, response.value).toJson());

        return Json(new { publishers = response.value }); //response contain the value.
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
