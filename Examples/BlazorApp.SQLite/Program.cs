using BlazorApp.SQLite.Data;
using BlazorApp.SQLite.Services;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.SQLite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<BlazorAppSQLiteContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("BlazorAppSQLiteContext") ?? throw new InvalidOperationException("Connection string 'SQLiteWebApiContext' not found.")));

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddTransient<CountryService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production
                // scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}