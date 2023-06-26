using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Netflix.Models;
using Netflix.Models.Inputs;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;


namespace Netflix.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly dynamic passwordHasher = new PasswordHasher<UserModel>();

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }


    [HttpPost]
    public IActionResult Signup([FromBody] SignupInpModel model)
    {
        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());

        FunctionResponse response = new AuthenticationService().CrateUser(model.Email, model.Password, model.FullName, model.Dob);
        if (!response.status)
            return Json(new HttpResponse(401, "Failed to create user").toJson());

        //generate token if succesfully created
        var token = GenerateJSONWebToken(model.Email, false); //admin can only register. So it is always false for signup method.

        return Json(new { generated_token = token });
    }



    [HttpPost]
    public IActionResult SignIn([FromBody] SignInInpModel model)
    {
        if (!ModelState.IsValid)
            return Json(new HttpResponse(401, "Insertion doesn't match with the Input Model").toJson());


        FunctionResponse response = new AuthenticationService().LoginUser(model.Email, model.Password);
        if (!response.status)
            return Json(new HttpResponse(401, response.value).toJson());

        //generate token if succesfully created
        var token = GenerateJSONWebToken(model.Email, response.value); //response contain isAdmin as value

        return Json(new { generated_token = token });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }



    /****
    
        ---- Generate web token.
    
    
    *****/

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





}
