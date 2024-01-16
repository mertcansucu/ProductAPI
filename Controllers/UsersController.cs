using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.DTO;
using ProductAPI.Models;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController: ControllerBase
    {
        private UserManager<AppUser> _userManager;

        public UsersController(UserManager<AppUser> userManager){
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(UserDTO model){
            if (!ModelState.IsValid)//ModelState hata var mı diye ilk onu kontrol ettim
            {
                return BadRequest(ModelState);
            }
            var user = new AppUser{
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email,
                DateAdded = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return StatusCode(201);//herhangi bir hata yoksa bu durum koduyla gönderdim
            }

            return BadRequest(result);//result succes değilse hata dönderdim
        }
    }
}