using Microsoft.AspNet.Identity.Owin;
using Portrack.Repositories.Identity;
using Portrack.Repositories.Application;
using System.Web;
using System.Web.Http;

namespace Portrack.Controllers.Application
{
    /// <summary>
    /// Base controller of Portrack. Responsible for the User manager and the IApplicationRepository.
    /// </summary>
    public class BaseController : ApiController
    {
        protected PortrackUserManager _userManager;
        protected readonly IApplicationRepository _repository;


        /// <summary>
        /// Method getting the user manager from the OwinContext.
        /// </summary>
        public PortrackUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<PortrackUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        /// <summary>
        /// Class constructor which injected 'IApplicationRepository' dependency.
        /// </summary>
        /// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
        public BaseController(IApplicationRepository repository)
        {
            _repository = repository;
        }
    }
}