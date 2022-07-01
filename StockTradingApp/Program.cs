using StockTradingApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
//enable session

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(60);//default timeout (log out) after 1 hour
});

builder.Services.AddSqlServer<sliceofbreadContext>(builder.Configuration.GetConnectionString("DefaultConnection"));


builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "XSRF-TOKEN";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
