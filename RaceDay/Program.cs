using RaceDay.Handlers;
using RaceDay.Services;
using RaceDay.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache(); // Add this line to register the distributed memory cache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(c => c.BaseAddress = new Uri(builder.Configuration["RaceDayWebAPI:BaseUrl"] !));

builder.Services.AddHttpContextAccessor(); // Add this line to register IHttpContextAccessor
builder.Services.AddTransient<JwtHandler>();

var apiBaseUrl = new Uri(builder.Configuration["RaceDayWebAPI:BaseUrl"] !);

builder.Services.AddHttpClient<IEventApiService, EventApiService>(client => client.BaseAddress = apiBaseUrl).AddHttpMessageHandler<JwtHandler>();
builder.Services.AddHttpClient<IEnrolmentApiService, EnrolmentApiService>(client => client.BaseAddress = apiBaseUrl).AddHttpMessageHandler<JwtHandler>();
builder.Services.AddHttpClient<IResultApiService, ResultApiService>(client => client.BaseAddress = apiBaseUrl).AddHttpMessageHandler<JwtHandler>();
builder.Services.AddHttpClient<ICategoryApiService, CategoryApiService>(client => client.BaseAddress = apiBaseUrl).AddHttpMessageHandler<JwtHandler>();
builder.Services.AddHttpClient<IProfileApiService, ProfileApiService>(c => c.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<JwtHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(    // default route SECOND
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
