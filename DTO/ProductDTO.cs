using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.DTO
{
    //DTO=>Data Transfer Object, DTo benim istediğim bilgilerin görünmesini sağlar, yani o objenin bilgilerini çağırdığımda sadece stediğim alanları gelir bu şekilde
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
    }
}