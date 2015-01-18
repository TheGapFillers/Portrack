using Microsoft.AspNet.Identity.Owin;
using Portrack.Repositories.AspAuth;
using Portrack.Repositories.Services;
using System.Web;
using System.Web.Http;

namespace Portrack.Controllers
{
    /// <summary>
    /// Base controller of Portrack. Responsible for the User manager and the IServicesRepository.
    /// </summary>
    public class BaseController : ApiController
    {
        protected AspAuthUserManager _userManager;
        protected readonly IServicesRepository _repository;


        /// <summary>
        /// Method getting the user manager from the OwinContext.
        /// </summary>
        public AspAuthUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<AspAuthUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        /// <summary>
        /// Class constructor which injected 'IServicesRepository' dependency.
        /// </summary>
        /// <param name="repository">Injected 'IServicesRepository' dependency.</param>
        public BaseController(IServicesRepository repository)
        {
            _repository = repository;
        }
    }
}