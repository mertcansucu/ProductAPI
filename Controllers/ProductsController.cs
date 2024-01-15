using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.DTO;
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

        private readonly ProductsContext _context;

        public ProductsController(ProductsContext context){
            /*veritanından bilgialeri aldığım için sildim
            _products = new List<Product>{
                new() { ProductId = 1, ProductName = "Iphone 14", Price = 60000, IsActive = true},
                new() { ProductId = 2, ProductName = "Iphone 15", Price = 70000, IsActive = true},
                new() { ProductId = 3, ProductName = "Iphone 16", Price = 80000, IsActive = true},
                new() { ProductId = 4, ProductName = "Iphone 17", Price = 90000, IsActive = true}
            };
            */
            _context = context;
        }

        //localhost:5000/api/products => GET burda tüm listeyi
        [HttpGet]
        public async Task<IActionResult> GetProducts(){
            // return _products ?? new List<Product>();//bunun kısayolu return _products == null ? new List<Product>() : _products

            /* bu haliylede istediklerimi çağırıyorum ama aşşağıda oluşturduğum fonk ile direk getiricem
            var products =  await _context
                            .Products.Where(i => i.IsActive).Select(p => 
                            //Where(i => i.IsActive) burda diyprum ki Isactive alanı ture olanları getir
                                    new ProductDTO{
                                        ProductId = p.ProductId,
                                        ProductName = p.ProductName,
                                        Price = p.Price
                            }).ToListAsync();
            */
            var products =  await _context
                            .Products.Where(i => i.IsActive).Select(p => ProductToDTO(p)).ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]//localhost:5000/api/products/1 => GET ben burda sadece nir elemanı geri getiriyorum
        public async Task<IActionResult> GetProduct(int? id){
            /*ben artık dönderilen değerlein durumuna göre hata mesajları göndericem mesela ben kayıtlı olmasa bile 12 id li birini göderince default olarak dönderiyor ve durum olarak 200 yani başarılı dönderiyor ama bunun ben böyle olmasını istemiyorum eğer yoksa mesela hata mesajını vermem lazım ki frontend ci bunları anlayabilsin
             return _products?.FirstOrDefault(i => i.ProductId == id) ?? new Product();//ben burda diyorum ki _products? null olmadığında içinde gez ve id si eşleşeni getir, ?? burda da diyorum ki eğer eşleşen yoksa null sa, new Product() boş olarak döndür(default değerleriyle)
            */

            if (id == null)
            {
                return NotFound();//burda hatayı dönderdim ama bunu kendim yazmak istersem şöyle yaparım return StatusCode(404,"")tırnaklar arsında istediğim meseajı yazıp cevabı dönderebilirim
            }

            var p =await _context
                            .Products
                            .Select(p => ProductToDTO(p))
                            .FirstOrDefaultAsync(i => i.ProductId == id);//_products? böyle yaparak products listesinin boş olmadığından emin oldum eğer boş değilse aldım bilgileri

            if (p == null)
            {
                return NotFound();
            }

            return Ok(p);//Ok bir hazır fonk. bana 200 kodu ile başarılı kaydı dönderir

            
        }

        [HttpPost]

        public async Task<IActionResult> CreateProduct(Product entity){
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct),new {id = entity.ProductId}, entity);//CreatedAtAction HTTP response status kodlarından 201 başarılı kodudur kayıt eklendi diye;201 durum koduyla başarılı olduğunu söylüyorum yani kayot eklendiğini ve eklendikten sonra kullanıcıya bir adres göndericez yani kullanıcıya hangi kişiyi eklediğimizin urlsini göndericez

            //mesela adres => http://localhost:5177/api/Products/6 
        }

        [HttpPut("{id}")]//localhost:5000/api/products/1 => Put ben burda id ile eşleşen elemanı güncelleme yapıcam
        public async Task<IActionResult> UpdateProduct(int id, Product entity){
            if (id != entity.ProductId)
            {
                return BadRequest();//400 nolu http status hatası,kullanıcı tarafından yanlış bir işlem yapıldığını gösterir
            }

            var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            product.ProductName = entity.ProductName;
            product.Price = entity.Price;
            product.IsActive = entity.IsActive;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }

            return NoContent(); //204 lü http status bir sorun olmadan çalıştığını söylüyorum ve geriye bir dönüş yaptırmıyorum
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int? id){
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }

            return NoContent(); //204 lü http status bir sorun olmadan çalıştığını söylüyorum ve geriye bir dönüş yaptırmıyorum 
        }

        private static ProductDTO ProductToDTO(Product p){//ben bunu oluşturup bilgileri çağırdığımda hangi bilgileri geleceğini söyledim, sonrasında o fonk. verip o şekilde çağırıcam
            return new ProductDTO//geri gelecek alan
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price
            };
        }
    }
}