using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
builder.Services.AddIdentity<AppUser,AppRole>().AddEntityFrameworkStores<ProductsContext>();//entity de kullanacağım kütüphaneler ve veritanıyla bağlantılı olan yeri ekledim(ProductsContext)

/*console eklemem gereken komutlar:
    dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 7.0.12
    dotnet ef migrations add IdentityTables
    dotnet ef database update

*/
builder.Services.Configure<IdentityOptions>(options =>{//identity default ayarlarını değiştirdim
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;

    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
});
builder.Services.AddAuthentication(x =>{//burada token kontrolü yapıcam doğru mu değil mi
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>{
    x.RequireHttpsMetadata = false;//taleplerin sadece https requestlerinden mi yani güvenli sorgular mı yapılsın diye soruyor bu zorunlu değil ben false diyip http adreslerindende gelsin dedim
    x.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuer = false,//bu apiyi kim yayınlamış,Issuer = "mertcansucu.com"
        ValidIssuer = "sadikturan.com",
        ValidateAudience = false,//hangi şirketler için geliştirdiğimi ip sağlandığını yazmam lazım
        ValidAudience = "",
        ValidAudiences = new string[] { "a","b"},
        
        
        
        ValidateIssuerSigningKey = true,//key bilgisini aktif ettim
        //alttaki bilgileri usercontrollerda token almadaki bilgileri entegre ettim
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            builder.Configuration.GetSection("AppSettings:Secret").Value ?? "")),
        ValidateLifetime = true //süre kontrolü
    };
});
builder.Services.AddControllers();//yalın bir controller bu biz burda oluşturduğumuz json dataları frontend geliştirici alıp görüntüye çevirebilsin diye json dataları oluşturucaz. Bunu mobil içinde kullanılabilir alıp kullanabilirler. Ben burda sevice oluşturdum.
//burdabki controller lar bir view ile bağlantılı olmayack yalın olarak oluşturucaz.Farklı uugulamlar arsındaki bağımlılığı kaldırmış olduk.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//api test dökümanları
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option => //swagger a Authentication butonu eklemiş oldum
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
//bu sayede ben js veya mobil uygulamamın içine ekleyeceğim request in headrına ekleyeceğim kodu aldım:
//Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwidW5pcXVlX25hbWUiOiJtZXJ0Y2Fuc3VjdSIsIm5iZiI6MTcwNTU4NTY4MCwiZXhwIjoxNzA1NjcyMDgwLCJpYXQiOjE3MDU1ODU2ODAsImlzcyI6Im1lcnRjYW5zdWN1LmNvbSJ9.NVEvY4i20tETidAbzzIKkyrQmlGpwnG5hatUFYzWB1s'

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())//burası bize projeyi geliştirmemi yayınlama alanında olduğumuzu söylüyoruz, geliştirme aşamasında olduğumuz için:appsettings.Development.json çalışır
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
