using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Database;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using MyFace.Services;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/interactions")]
    public class InteractionsController : ControllerBase
    {
        private readonly IInteractionsRepo _interactions;
        private readonly IUsersRepo _users;


        public InteractionsController(IInteractionsRepo interactions, IUsersRepo users)
        {
            _interactions = interactions;
            _users = users;
        }
    
        [HttpGet("")]
        public ActionResult<ListResponse<InteractionResponse>> Search([FromQuery] SearchRequest search)
        {
            var interactions = _interactions.Search(search);
            var interactionCount = _interactions.Count(search);
            return InteractionListResponse.Create(search, interactions, interactionCount);
        }

        [HttpGet("{id}")]
        public ActionResult<InteractionResponse> GetById([FromRoute] int id)
        {
            var interaction = _interactions.GetById(id);
            return new InteractionResponse(interaction);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateInteractionRequest newUser, [FromHeader(Name = "Authorization")] string authorisationHeader)
        {
            var authenticator = new AuthService(_users);

            if (authenticator.Authenticate(authorisationHeader)){

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                    string encodedAuthHeader = authorisationHeader.Substring("Basic ".Length).Trim();

                    string usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedAuthHeader));

                    int seperate = usernamePassword.IndexOf(":");

                    string decodedUsername = usernamePassword.Substring(0, seperate);

                    User searchedUser = _users.GetByUsername(decodedUsername);
            
                var interaction = _interactions.Create(newUser, searchedUser.Id);

                var url = Url.Action("GetById", new { id = interaction.Id });
                var responseViewModel = new InteractionResponse(interaction);
                return Created(url, responseViewModel);
            }
        return new UnauthorizedResult();
        }
            
        

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _interactions.Delete(id);
            return Ok();
        }
    }
}
