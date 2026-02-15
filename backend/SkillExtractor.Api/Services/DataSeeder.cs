using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillExtractor.Api.Data;
using SkillExtractor.Api.Models;

namespace SkillExtractor.Api.Services
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp, IWebHostEnvironment env)
        {
            using var scope = sp.CreateScope();
            var services = scope.ServiceProvider;

            var db = services.GetRequiredService<AppDbContext>();

            // Apply pending migrations
            await db.Database.MigrateAsync();

            // Seed default skills only if none exist
            if (await db.Skills.AnyAsync())
                return;

            var skills = new List<Skill>
            {
                new Skill { Name = "C#" },
                new Skill { Name = ".NET", Aliases = "dotnet, dot net" },
                new Skill { Name = "ASP.NET Core", Aliases = "aspnet, asp.net" },
                new Skill { Name = "SQL" },
                new Skill { Name = "MS SQL Server", Aliases = "mssql, sql server" },
                new Skill { Name = "EF Core" },
                new Skill { Name = "React" },
                new Skill { Name = "TypeScript" },
                new Skill { Name = "Docker" },
                new Skill { Name = "Azure" }
            };

            db.Skills.AddRange(skills);
            await db.SaveChangesAsync();
        }
    }
}
