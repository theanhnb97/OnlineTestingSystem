namespace TestingSystem.Controllers
{
    using System.Web.Mvc;
    using TestingSystem.Common;
    using TestingSystem.DataTranferObject;
    using TestingSystem.Sevice;

    /// <summary>
    /// Defines the <see cref="_RegisterController" />
    /// </summary>
    public class _RegisterController : Controller
    {
        /// <summary>
        /// Defines the userService
        /// </summary>
        protected IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="_RegisterController"/> class.
        /// </summary>
        /// <param name="userService">The userService<see cref="IUserService"/></param>
        public _RegisterController(IUserService userService)
        {
            this.userService = userService;
        }

        //public RegisterController(IUserService userService) : base(userService)
        //{
        //}

        // GET: Register
        /// <summary>
        /// The Index
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// The Index
        /// </summary>
        /// <param name="user">The user<see cref="UserRegister"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost]
        public ActionResult Index(UserRegister user)
        {
            if (user.password != user.comfirmPassword)
                return View();
            user.password = Encryptor.MD5Hash(user.password);
            if (userService.Register(user))
                return RedirectToAction("Login", "Login");
            return View();
        }
    }
}
