// Services/IAServiceMock.cs
using System.Text.Json;

namespace FlowMind.Api.Services
{
    public class IAServiceMock : IIAService
    {
        public async Task<string> GerarRotinaAsync(string prompt)
        {
            await Task.Delay(300);

            var blocos = new[]
            {
                new { inicio = "09:00", fim = "09:50", tipo = "foco", titulo = "Tarefa principal", duracao = 50 },
                new { inicio = "09:50", fim = "09:53", tipo = "pausa", titulo = "Alongamento", duracao = 3 },
                new { inicio = "09:53", fim = "10:43", tipo = "foco", titulo = "Reunião", duracao = 50 },
                new { inicio = "17:00", fim = "17:15", tipo = "recompensa", titulo = "Café + música", duracao = 15 }
            };

            var json = JsonSerializer.Serialize(new
            {
                blocos,
                totalFoco = 150,
                totalPausas = 18
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return json;
        }

        public async Task<string> GerarFeedbackAsync(string prompt)
        {
            await Task.Delay(200);
            return "Você manteve pausas regulares. Continue assim!";
        }
    }
}