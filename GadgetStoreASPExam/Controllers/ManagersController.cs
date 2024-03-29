﻿using GadgetStoreASPExam.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GadgetStoreASPExam.Roles;
using System.Security.Claims;

namespace GadgetStoreASPExam.Controllers
{
    [ApiController, Authorize]
    [Route("api/[controller]")]
    public class ManagersController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public ManagersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        [Route("getUserRole")]
        public IActionResult GetUserRole()
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == null)
            {
                return BadRequest("Unable to determine user's role");
            }

            return Ok(new { role });
        }

        [HttpGet("UserId")]
        public async Task<IActionResult> GetUserId()
        {
            var userNameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (userNameClaim == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByNameAsync(userNameClaim);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user.UserName);
        }


        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [Route("delManager")]
        public async Task<IActionResult> DelManager([FromBody] Register model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return NotFound($"User with username {model.UserName} not found");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok($"User with username {model.UserName} deleted successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting user with username {model.UserName}");
            }
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        [Route("selectUser")]
        public async Task<ActionResult> Get()
        {
            var listUser = _userManager.Users;
            if(listUser == null && listUser.Count() < 0)
            {
                return NotFound("Error");
            }
            return Ok(listUser);
        }
    }
}
