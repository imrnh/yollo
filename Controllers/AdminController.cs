using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Netflix.Models;

namespace Netflix.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }

    public IActionResult Dashboard()
    {
        return Json(new { controller_name = "Admin" });
        // return View();
    }

    [HttpPost]
    public IActionResult AddGenre([FromBody] GenreInputModel model)
    {

        if (!ModelState.IsValid)
        {
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());
        }

        ACreateGenre.InsertGenre(model.Name);
        return Json(new HttpResponse(200, "Genre '" + model.Name + "' added succesfully").toJson());
    }

    [HttpPost]
    public IActionResult AddPublisher([FromBody] PublisherInputModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());
        }

        ACreatePublisher.InsertPublisher(model.Name);

        return Json(new HttpResponse(200, "Publisher '" + model.Name + "' added succesfully" ).toJson());
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
