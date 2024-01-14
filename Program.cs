using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ProductsContext>(x => x.UseSqlite("Data Source = products.db"));
/* bu işlemden sonra migrations oluşturucam, onun kodları
    
     dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 7.0.12
    
    dotnet add package Microsoft.EntityFrameworkCore.Design --version 7.0.12

    dotnet build
    dotnet tool update --global dotnet-ef --version 7.0.12 versiyonu güncelledim
    dotnet ef migrations add InitialCreate
    dotnet ef database update
 

*/

builder.Services.AddControllers();//yalın bir controller bu biz burda oluşturduğumuz json dataları frontend geliştirici alıp görüntüye çevirebilsin diye json dataları oluşturucaz. Bunu mobil içinde kullanılabilir alıp kullanabilirler. Ben burda sevice oluşturdum.
//burdabki controller lar bir view ile bağlantılı olmayack yalın olarak oluşturucaz.Farklı uugulamlar arsındaki bağımlılığı kaldırmış olduk.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//api test dökümanları
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())//burası bize projeyi geliştirmemi yayınlama alanında olduğumuzu söylüyoruz, geliştirme aşamasında olduğumuz için:appsettings.Development.json çalışır
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
