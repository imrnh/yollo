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
        return Json(new {controller_name="Admin"});
        // return View();
    }

    [HttpPost]
    public IActionResult AddGenre([FromBody] GenreInputModel model)
    {
        // Check if the model state is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Pass the name value to the InsertGenre method
        ACreateGenreAndPublisher.InsertGenre(model.Name);

        // Return a success response
        return Ok("Genre added successfully");
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
