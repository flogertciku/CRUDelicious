using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CRUDelicious.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;


namespace CRUDelicious.Controllers;
// Name this anything you want with the word "Attribute" at the end
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Auth", "Home", null);
        }
    }
}


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;  

    public HomeController(ILogger<HomeController> logger,MyContext context)
    {
        _logger = logger;
        _context = context;
    }
    [SessionCheck]
    public IActionResult Index()
    {
        ViewBag.listaMeDish = _context.Dishes.ToList();
        return View();
    }

    [HttpGet("Auth")]
    public IActionResult Auth(){
        int justANR =3;
        return View("Auth");
    }
    [HttpPost("Register")]
    public IActionResult Register(User useriNgaForma){
            
            if (ModelState.IsValid)
            {   
                PasswordHasher<User> Hasher = new PasswordHasher<User>();   
            // Updating our newUser's password to a hashed version         
            useriNgaForma.Password = Hasher.HashPassword(useriNgaForma, useriNgaForma.Password); 
            _context.Add(useriNgaForma);
            _context.SaveChanges();
            // User useriNgaDB = _context.Users.First(e=> e.Email == useriNgaForma.Email);
            // //                      int  UserId = useriNgaDb.userID
            // HttpContext.Session.SetInt32("UserId", useriNgaDB.UserId);

                
                return RedirectToAction("Auth");
            }
            return View("Auth");

    }
    [HttpPost("Login")]
    public IActionResult Login(Login useriNgaForma){
            
            if (ModelState.IsValid)
            {   
            
            User useriNgaDB = _context.Users.FirstOrDefault(e=> e.Email == useriNgaForma.LoginEmail);
            if(useriNgaDB == null)        
        {            
            // Add an error to ModelState and return to View!            
            ModelState.AddModelError("LoginEmail", "Invalid Email");            
            return View("Auth");        
        } 
            // //                      int  UserId = useriNgaDb.userID
            // HttpContext.Session.SetInt32("UserId", useriNgaDB.UserId);


            PasswordHasher<Login> hasher = new PasswordHasher<Login>(); 
              var result = hasher.VerifyHashedPassword(useriNgaForma, useriNgaDB.Password, useriNgaForma.LoginPassword);
            if(result == 0)        
        {            
                ModelState.AddModelError("LoginPassword", "Invalid Password");            
            return View("Auth");  
        }
                
                HttpContext.Session.SetInt32("UserId", useriNgaDB.UserId);
                return RedirectToAction("Index");
                
            }

            
            return View("Auth");

    }
    [HttpGet("Logout")]
    public IActionResult Logout(){
        HttpContext.Session.Clear();
        return RedirectToAction("Auth");
    }
    [SessionCheck]
    [HttpGet("AddDish")]
    public IActionResult AddDish(){
        return View();
    }
    [SessionCheck]
    [HttpPost("CreateDish")]
    public IActionResult CreateDish(Dish dishNgaForma){

        if (ModelState.IsValid)
        {
            _context.Add(dishNgaForma);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View("AddDish");

    }
    [SessionCheck]
    [HttpGet("Dish/{id}")]
    public IActionResult Dish(int id){
        Dish gatimi = _context.Dishes.FirstOrDefault(e => e.DishId == id);
        return View(gatimi);

    }
    [SessionCheck]
    [HttpGet("Delete/{id}")]
    public IActionResult Delete(int id){
        Dish gatimiQeDoFshi = _context.Dishes.FirstOrDefault(e=> e.DishId == id);
        _context.Remove(gatimiQeDoFshi);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
