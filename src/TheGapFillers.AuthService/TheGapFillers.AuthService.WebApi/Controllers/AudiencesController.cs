using System.Web.Http;
using TheGapFillers.Auth.Models;
using TheGapFillers.Auth.Repositories;

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

            Audience newAudience = CustomUserManager.AddAudience(audienceModel.Name);

            return Ok<Audience>(newAudience);

        }
    }
}
