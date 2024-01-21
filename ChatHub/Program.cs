using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Components;
using ChatShared.Models;
using Microsoft.AspNetCore.SignalR;
using ChatHub.Hubs;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        

        builder.Services.AddSignalR();
        //builder.Services.AddSingleton<IUserIdProvider,NameUserIdProvider>();

        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" });
        });

        builder.Services.AddSingleton<Dictionary<string,User>>();

        builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer(option =>
      option.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
    }
    );

        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseWebAssemblyDebugging();
        }

        app.UseBlazorFrameworkFiles();

        app.UseResponseCompression();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();
       
        app.MapHub<ChatHub.Hubs.ChatHub>("/chatHub");
        app.MapHub<ChatHub.Hubs.MessageHub>("/messagehub");
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}

