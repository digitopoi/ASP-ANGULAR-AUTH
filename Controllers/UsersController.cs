using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ASP_Angular_Auth.Services;
using ASP_Angular_Auth.Models.ResourceModels;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using ASP_Angular_Auth.Models;
using Microsoft.AspNetCore.Authorization;
using ASP_Angular_Auth.Data;

namespace ASP_Angular_Auth.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserResource userResource)
        {
            var user = _userService.Authenticate(userResource.UserName, userResource.Password);

            if (user == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //  return basic user info (without password) and token to store client-side
            return Ok(new {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody]UserResource userResource)
        {
            //  map resource to model
            var user = _mapper.Map<User>(userResource);

            try
            {
                //  save
                _userService.Create(user, userResource.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                //  return error message if there was an exception
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var userResources = _mapper.Map<IList<UserResource>>(users);
            return Ok(userResources);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            var userResource = _mapper.Map<UserResource>(user);
            return Ok(userResource);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UserResource userResource)
        {
            //  map resource to model
            var user = _mapper.Map<User>(userResource);
            user.Id = id;

            try
            {
                //  save
                _userService.Update(user, userResource.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                //  return error message if there was an exception
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }
    }
}