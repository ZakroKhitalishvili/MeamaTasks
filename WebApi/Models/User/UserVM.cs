using System.ComponentModel.DataAnnotations;

namespace Meama_Tasks.Models.User;

public class UserVM

{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
}