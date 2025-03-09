using Microsoft.EntityFrameworkCore;
using Project_PRN222.Models;
using Project_PRN222.Repositories.Implementations;
using Project_PRN222.Repositories.Interfaces;
using Project_PRN222.Services.Implementations;
using Project_PRN222.Services.Interfaces;
using Project_PRN222.Services; // Th�m namespace cho VnpayPayment

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ProjectPrn222Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DB")));

// Add session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();       // Th�m cho Order
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>(); // Th�m cho OrderItem
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();   // Th�m cho Delivery

// Register services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IOrderService, OrderService>();       // Th�m cho OrderService
builder.Services.AddScoped<VnpayPayment>();                      // Th�m cho VNPAY

// Add HttpContextAccessor for session and IP address retrieval
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();