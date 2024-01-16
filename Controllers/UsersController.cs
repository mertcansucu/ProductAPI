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
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UsersController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager){
            _userManager = userManager;
            _signInManager = signInManager;
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

        public async Task<IActionResult> Login(LoginDTO model){
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest(new {message = "Email Hatalı!"});
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password,false);//burdaki 3. parametre olan false programcs de buraya eklediğim 5 kere hatalı girebilirsin gibi kuralları aktif etmek istiyor musun diyor yok dedim bende

            if (result.Succeeded)
            {
                return Ok(
                    new { token = "token"}
                );
            }
            return Unauthorized();//403 nolu hatadır yetkiniz yok diyorum
        }
    }
}