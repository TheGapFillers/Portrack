using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using System.Web.Http;
using TheGapFillers.Portrack.Models.Identity;
using TheGapFillers.Portrack.Repositories.Identity;

namespace TheGapFillers.AuthService.WebApi.Controllers
{
    [RoutePrefix("api/audiences")]
    public class AudiencesController : ApiController
    {
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(AudienceModel audienceModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //PortrackUserManager userManager = Request.GetOwinContext().GetUserManager<PortrackUserManager>();
            Audience newAudience = CustomUserManager.AddAudience(audienceModel.Name);

            return Ok<Audience>(newAudience);

        }
    }
}
