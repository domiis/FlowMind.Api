using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowMind.Api.Models;

[Table("WEATHER_FORECASTS")] // Nome da tabela no Oracle
public class WeatherForecast
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}