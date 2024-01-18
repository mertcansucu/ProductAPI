using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IConfiguration _configuration;//appsettings.development de oluşturduğum key bilgisini almak için ekledim

        public UsersController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,IConfiguration configuration){
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
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

        [HttpPost("login")]
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
                    new { token = GenerateJWT(user)}
                );
            }
            return Unauthorized();//403 nolu hatadır yetkiniz yok diyorum
        }

        private object GenerateJWT(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value ?? "");//key bilgisini aldım ama byte cinsinden aldım ve eğer boş gelirse de boş gönder dedim
            var tokenDescriptor = new SecurityTokenDescriptor{//token ı ürettim yani bölümlerini üreticem
                Subject = new ClaimsIdentity(//token ını içine ekleyeceğim bilgiler
                    new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    }
                ),
                Expires = DateTime.UtcNow.AddDays(1),//ne kadar süre kalacağını söyledim
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature),//token şifreleme algoritması
                Issuer = "mertcansucu.com"
                
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);//token bilgisi ürettim
            return tokenHandler.WriteToken(token);
        }
    }
}