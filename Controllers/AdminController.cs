using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Netflix.Models;

namespace Netflix.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

    private string GenerateJSONWebToken(string username, bool isAdmin)
    {
        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("yollo@yollo87787878"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
        new Claim("Issuer", "yollo"),
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, isAdmin? "admin" : "user"),
        new Claim(JwtRegisteredClaimNames.UniqueName, username)
    };


        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken("yollo", "yollo",
            claims, expires: DateTime.Now.AddDays(15),
            signingCredentials: credentials);


        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

    }


    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    public IActionResult Dashboard()
    {
        return Json(new { controller_name = "Admin" });
    }


    public IActionResult GetToken()
    {
        return Json(new { token = GenerateJSONWebToken("imran", true)});
    }



    [Authorize(Roles = "admin")]
    public IActionResult AllGenres()
    {
        List<GenreModel> genres = new AGenreServices().GetAllGenres();
        return Json(new {all_genres = genres });
    }

    public IActionResult AllPublishers()
    {
        List<PublisherModel> publishers = new APublisherServices().GetAllPublishers();
        return Json(new { all_publishers = publishers });
    }


    [HttpPost]
    public IActionResult AddGenre([FromBody] GenreInputModel model)
    {

        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());

        FunctionResponse response = new AGenreServices().InsertGenre(model.Name);
        if (!response.status)
            return Json(new HttpResponse(401, response.value).toJson());

        return Json(new HttpResponse(200, "Genre '" + model.Name + "' added succesfully").toJson());
    }


    [HttpPost]
    public IActionResult AddPublisher([FromBody] PublisherInputModel model)
    {
        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());

        FunctionResponse response = new APublisherServices().InsertPublisher(model.Name);
        if (!response.status)
            return Json(new HttpResponse(401, response.value).toJson());

        return Json(new HttpResponse(200, "Publisher '" + model.Name + "' added succesfully").toJson());
    }


    [HttpPost]
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
