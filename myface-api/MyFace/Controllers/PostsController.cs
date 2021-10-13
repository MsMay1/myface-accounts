using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using MyFace.Models.Database;
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MyFace.Services;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace MyFace.Controllers
{

    [ApiController]
    [Route("/posts")]
    public class PostsController : ControllerBase
    {
        private readonly IPostsRepo _posts;
        private readonly IUsersRepo _users;


        public PostsController(IPostsRepo posts, IUsersRepo users)
        {
            _posts = posts;
            _users = users;

        }

        [HttpGet("")]
        public ActionResult<PostListResponse> Search(
            [FromQuery] PostSearchRequest searchRequest,
            [FromHeader(Name = "Authorization")] string authorisationHeader)
        {
            var authenticator = new AuthService(_users);

            if (authenticator.Authenticate(authorisationHeader))
            {
                var posts = _posts.Search(searchRequest);
                var postCount = _posts.Count(searchRequest);
                return PostListResponse.Create(searchRequest, posts, postCount);
            }

            return new UnauthorizedResult();

        }



        [HttpGet("{id}")]
        public ActionResult<PostResponse> GetById([FromRoute] int id, [FromHeader(Name = "Authorization")] string authorisationHeader)
        {
            var authenticator = new AuthService(_users);

            if (authenticator.Authenticate(authorisationHeader))
            {
                var post = _posts.GetById(id);
                return new PostResponse(post);
            }
            return new UnauthorizedResult();
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreatePostRequest newPost, [FromHeader(Name = "Authorization")] string authorisationHeader)
        {
            var authenticator = new AuthService(_users);

            if (authenticator.Authenticate(authorisationHeader))
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string encodedAuthHeader = authorisationHeader.Substring("Basic ".Length).Trim();

                // Encode and return hash
                string usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedAuthHeader));

                int seperate = usernamePassword.IndexOf(":");

                string decodedUsername = usernamePassword.Substring(0, seperate);

                User searchedUser = _users.GetByUsername(decodedUsername);

                var post = _posts.Create(newPost, searchedUser.Id);

                var url = Url.Action("GetById", new { id = post.Id });

                var postResponse = new PostResponse(post);
                return Created(url, postResponse);
            }

            return new UnauthorizedResult();
        }

        [HttpPatch("{id}/update")]
        public ActionResult<PostResponse> Update([FromRoute] int id, [FromBody] UpdatePostRequest update, [FromHeader(Name = "Authorization")] string authorisationHeader)
        {
            var authenticator = new AuthService(_users);

            if (authenticator.Authenticate(authorisationHeader))
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var post = _posts.Update(id, update);
                return new PostResponse(post);
            }
            return new UnauthorizedResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id, [FromHeader(Name = "Authorization")] string authorisationHeader)
        {
            var authenticator = new AuthService(_users);

            if (authenticator.Authenticate(authorisationHeader))
            {
                _posts.Delete(id);
                return Ok();
            }

            return new UnauthorizedResult();
        }
    }
}