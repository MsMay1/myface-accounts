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
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepo _users;

        public UsersController(IUsersRepo users)
        {
            _users = users;
        }

        [HttpGet("")]
        public ActionResult<UserListResponse> Search([FromQuery] UserSearchRequest searchRequest)
        {
            var users = _users.Search(searchRequest);
            var userCount = _users.Count(searchRequest);
            return UserListResponse.Create(searchRequest, users, userCount);
        }

        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetById([FromRoute] int id)
        {
            var user = _users.GetById(id);
            return new UserResponse(user);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserRequest newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _users.Create(newUser);

            var url = Url.Action("GetById", new { id = user.Id });
            var responseViewModel = new UserResponse(user);
            return Created(url, responseViewModel);
        }

        [HttpPatch("{id}/update")]
        public ActionResult<UserResponse> Update([FromRoute] int id, [FromBody] UpdateUserRequest update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _users.Update(id, update);
            return new UserResponse(user);
        }

        [HttpPatch("{id}/updaterole")]
        public ActionResult UpdateRole([FromRoute] int id, [FromHeader(Name = "Authorization")] string authorisationHeader)
        {
            var authenticator = new AuthService(_users);

            if (authenticator.Authenticate(authorisationHeader))
            {
                string encodedAuthHeader = authorisationHeader.Substring("Basic ".Length).Trim();

                string usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedAuthHeader));

                int seperate = usernamePassword.IndexOf(":");

                string decodedUsername = usernamePassword.Substring(0, seperate);

                User searchedUser = _users.GetByUsername(decodedUsername);

                if (searchedUser.Type == Role.ADMIN)
                {
                    _users.UpdateRole(id);
                    return Ok();
                }

                return StatusCode(403, "unauthorised access");
            }

            return new UnauthorizedResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _users.Delete(id);
            return Ok();
        }
    }
}