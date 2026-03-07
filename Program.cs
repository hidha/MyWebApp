using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
// set EPPlus license context globally
ExcelPackage.License.SetNonCommercialPersonal("Hidha Kaleelurrahman");

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
