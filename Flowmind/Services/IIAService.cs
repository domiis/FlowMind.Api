// Services/IIAService.cs
namespace FlowMind.Api.Services
{
    public interface IIAService
    {
        Task<string> GerarRotinaAsync(string prompt);
        Task<string> GerarFeedbackAsync(string prompt);
    }
}