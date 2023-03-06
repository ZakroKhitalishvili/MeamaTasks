using System.ComponentModel.DataAnnotations;

namespace Meama_Tasks.Models.User;

public class UserUpdateVM

{
    [Required]
    public required string Id { get; set; }
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Name { get; set; }
    public string? Password { get; set; }
    public string? NewPassword { get; set; }
    [Compare("NewPassword")]
    public string? ConfirmNewPassword { get; set; }
    [Required]
    public required string Role { get; set; }
}