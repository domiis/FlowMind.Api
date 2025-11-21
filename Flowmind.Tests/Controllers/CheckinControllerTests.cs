using System.Net;
using System.Net.Http.Json;
using FlowMind.Api.DTOs.Request;
using FlowMind.Tests.Factories;
using Flowmind.Models; 
using Xunit;
using FlowMind.Api.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FlowMind.Tests.Controllers
{
    public class CheckinControllerTests
    {
        [Fact]
        public async Task Deve_Criar_Checkin_Com_Sucesso()
        {
            // Arrange
            var app = FlowMindApplicationFactory.Create();
            var client = app.CreateClient();

            var userId = 1;

            // Criando usuário fake no banco em memória
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Users.Add(new User
                {
                    Id = userId,
                    Name = "Teste",
                    Email = "teste@teste.com",
                    PasswordHash = "123"
                });

                db.SaveChanges();
            }

            var request = new CheckinCreateRequest
            {
                Humor = FlowMind.Api.Models.Entities.Humor.Calmo,
                Energia = FlowMind.Api.Models.Entities.Energia.Media,
                Sono = FlowMind.Api.Models.Entities.Sono.Bom
            };

            // Act
            var response = await client.PostAsJsonAsync(
                $"api/users/{userId}/checkins",  // 👈 sem /v1 porque seu controller não usa versionamento na rota
                request
            );

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
