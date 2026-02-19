using blog_community_api.Data;
using blog_community_api.Data.Repositories;
using blog_community_api.Data.Entities;
using blog_community_api.Mapping;
using AutoMapper;
using blog_community_api.Security;
using Microsoft.EntityFrameworkCore;

namespace blog_community_api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        
        // Data DIs
        builder.Services.AddDbContext<BlogContext>
        (
            options => options.UseNpgsql(builder.Configuration.GetConnectionString("BlogDatabase"))
        );
        builder.Services.AddScoped<IRepository<User>, UserRepository>();
        builder.Services.AddScoped<IRepository<BlogPost>, BlogPostRepository>();
        builder.Services.AddScoped<IRepository<Category>, CategoryRepository>();
        builder.Services.AddScoped<IRepository<Comment>, CommentRepository>();
        builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);
        
        // Security DIs
        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}