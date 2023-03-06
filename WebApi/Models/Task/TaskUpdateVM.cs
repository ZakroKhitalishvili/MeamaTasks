
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Meama_Tasks.Models.User;

namespace Meama_Tasks.Models.Task;

public class TaskUpdateVM
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public IFormFileCollection? Files { get; set; }
    public IList<string> DeletedFiles { get; set; }
}