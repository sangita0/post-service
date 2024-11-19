using Microsoft.AspNetCore.Mvc;

namespace post_microservice.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
