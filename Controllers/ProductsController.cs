using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProductAPI.Controllers
{
    //localhost:5177/api/products([controller] desemde aynı şey)
    [ApiController]//bu controller ın da bir route a ihtiyacı var,öünkğ hangi adresrten erişeceğini söylemem lazım
    [Route("api/[controller]")]
    public class ProductsController: ControllerBase
    {
        private static readonly string[] Products ={//products listesi oluşturdum
            "Iphone 14", "Iphone 15", "Iphone 16"
        };

        //localhost:5000/api/products => GET burda tüm listeyi
        [HttpGet]
        public string[] GetProducts(){
            return Products;
        }

        [HttpGet("{id}")]//localhost:5000/api/products/1 => GET ben burda sadece nir elemanı geri getiriyorum
        public string GetProduct(int id){
            return Products[id];
        }
    }
}