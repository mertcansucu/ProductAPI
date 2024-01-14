using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;

namespace ProductAPI.Controllers
{
    //localhost:5177/api/products([controller] desemde aynı şey)
    [ApiController]//bu controller ın da bir route a ihtiyacı var,öünkğ hangi adresrten erişeceğini söylemem lazım
    [Route("api/[controller]")]
    public class ProductsController: ControllerBase
    {
        // private static readonly string[] Products ={//products listesi oluşturdum
        //     "Iphone 14", "Iphone 15", "Iphone 16"
        // };

        //model üzerinden çağrıp bilgileri alıcam

        private static List<Product>? _products;

        public ProductsController(){
            _products = new List<Product>{
                new() { ProductId = 1, ProductName = "Iphone 14", Price = 60000, IsActive = true},
                new() { ProductId = 2, ProductName = "Iphone 15", Price = 70000, IsActive = true},
                new() { ProductId = 3, ProductName = "Iphone 16", Price = 80000, IsActive = true},
                new() { ProductId = 4, ProductName = "Iphone 17", Price = 90000, IsActive = true}
            };
        }

        //localhost:5000/api/products => GET burda tüm listeyi
        [HttpGet]
        public List<Product> GetProducts(){
            return _products ?? new List<Product>();//bunun kısayolu return _products == null ? new List<Product>() : _products
        }

        [HttpGet("{id}")]//localhost:5000/api/products/1 => GET ben burda sadece nir elemanı geri getiriyorum
        public Product GetProduct(int id){
            return _products?.FirstOrDefault(i => i.ProductId == id) ?? new Product();//ben burda diyorum ki _products? null olmadığında içinde gez ve id si eşleşeni getir, ?? burda da diyorum ki eğer eşleşen yoksa null sa, new Product() boş olarak döndür(default değerleriyle)
        }
    }
}