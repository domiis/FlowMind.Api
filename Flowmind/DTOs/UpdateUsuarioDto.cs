using System.ComponentModel.DataAnnotations;

namespace FlowMind.Api.Dtos;

public class UpdateUserDto
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    [Required] public string UserType { get; set; } = string.Empty;
}