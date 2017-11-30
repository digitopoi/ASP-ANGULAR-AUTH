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

        public UsersController(
            IUserService userService,
            IMapper mapper)
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

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret)
        }
    }
}