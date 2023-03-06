using System.ComponentModel.DataAnnotations;

namespace Meama_Tasks.Models.User;

public class UserCreateVM

{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string Password { get; set; }
    [Required]
    [Compare("Password")]
    public required string ConfirmPassword { get; set; }
    [Required]
    public required string Role { get; set; }
}