using System.ComponentModel.DataAnnotations;

namespace FlowMind.Api.Dtos;

public class CreateUserDto
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string PasswordHash { get; set; } = string.Empty;
    [Required] public string UserType { get; set; } = "User";
}