using SMS.Web;
using SMS.Data.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the DI container.
builder.Services.AddSingleton<IUserService,UserServiceDb>();
builder.Services.AddScoped<IStudentService,StudentServiceDb>();

// ** add authentication service using Cookie Scheme **
builder.Services.AddCookieAuthentication();

// add mvc support                             
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
} 
else
{
    // in development mode seed the database each time the application starts  
    using (var scope = app.Services.CreateScope())
    {
        ServiceSeeder.Seed(
            scope.ServiceProvider.GetService<IUserService>(), 
            scope.ServiceProvider.GetService<IStudentService>()
        );
    }
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
