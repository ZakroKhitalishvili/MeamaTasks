namespace Meama_Tasks.Models.Auth;

public class AuthResponseVM
{
    public required string Token { get; set; }
    public DateTime Expiration { get; set; }
}