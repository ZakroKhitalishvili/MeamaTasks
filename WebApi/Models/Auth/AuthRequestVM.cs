using Microsoft.Build.Framework;

namespace Meama_Tasks.Models.Auth;

public class AuthRequestVM
{
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
}