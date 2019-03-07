namespace TestingSystem.Controllers
{
    using System.Web.Mvc;
    using TestingSystem.Common;
    using TestingSystem.Sevice;

    /// <summary>
    /// Defines the <see cref="_AccountController" />
    /// </summary>
    public class _AccountController : Controller
    {
        /// <summary>
        /// Defines the userService
        /// </summary>
        private IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="_AccountController"/> class.
        /// </summary>
        /// <param name="userService">The userService<see cref="IUserService"/></param>
        public _AccountController(IUserService userService)
        {
            this.userService = userService;
        }

        //GET: Account
        /// <summary>
        /// The Register
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        public ActionResult Register()
        {
            return View();
        }

        //[HttpGet]
        //public ActionResult Verify()
        //{
        //    return RedirectToAction("Index", "Home");
        //}
        //[HttpPost]
        /// <summary>
        /// The Verify
        /// </summary>
        /// <param name="key">The key<see cref="string"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        public ActionResult Verify(string key)
        {
            key = Base64.Decode(key);
            if (userService.Active(key) == 1)
                return RedirectToAction("Login", "Login");
            else
            {
                return View();
            }
        }

        /// <summary>
        /// The Logout
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        public ActionResult Logout()
        {
            Session.Remove("Name");
            return RedirectToAction("Login", "Login");
        }
    }
}
