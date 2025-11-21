using FlowMind.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace FlowMind.Tests.Factories
{
    public class FlowMindApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //
                // 1️⃣ REMOVE QUALQUER DbContextOptions<AppDbContext>
                //
                var descriptors = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                             || d.ServiceType == typeof(DbContextOptions))
                    .ToList();

                foreach (var d in descriptors)
                    services.Remove(d);

                //
                // 2️⃣ REMOVE AppDbContext registrado anteriormente
                //
                var dbContextDescriptors = services
                    .Where(d => d.ServiceType == typeof(AppDbContext))
                    .ToList();

                foreach (var d in dbContextDescriptors)
                    services.Remove(d);

                //
                // 3️⃣ ADICIONA APENAS O PROVIDER INMEMORY
                //
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("FlowMindTestsDB");
                });

                //
                // 4️⃣ GARANTE QUE O BANCO É CRIADO ANTES DO TESTE RODAR
                //
                using var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            });
        }

        public static FlowMindApplicationFactory Create() => new();
    }
}
