using System.Collections.Generic;
using System.Linq;
using MyFace.Models.Database;
using MyFace.Models.Request;
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using MyFace.Repositories;
using Microsoft.AspNetCore.Http;

namespace MyFace.Services
{

    // add interface
    public class AuthService //static?
    {

        private readonly IUsersRepo _users;

        public AuthService(IUsersRepo users)
        {
            _users = users;
        }

        public bool Authenticate(string authorisationHeader)
        {

            // string authorisationHeader = request.Headers["Authorization"];

            if (authorisationHeader == null)
                return false;



            // Remove basic(find length of basic and split there) and trim white space
            string encodedAuthHeader = authorisationHeader.Substring("Basic ".Length).Trim();

            // Encode and return hash
            string usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedAuthHeader));


            //Seperate at : and split before and after for username and password
            int seperate = usernamePassword.IndexOf(':');
            string decodedUsername = usernamePassword.Substring(0, seperate);
            string decodedPassword = usernamePassword.Substring(seperate + 1);
            // split here       
            // search user in database

            User searchedUser = _users.GetByUsername(decodedUsername);
            // check if user exists and hash password - does this create a new hash and therefore it would not match?

            if (searchedUser == null)
                return false;

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: decodedPassword,
                salt: searchedUser.Salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            if (searchedUser.Hashed_password == hashed)
            {
                return true;
            }

            return false;

        }

    }
}
