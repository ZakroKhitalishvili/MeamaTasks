
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Meama_Tasks.Models.User;

namespace Meama_Tasks.Models.Task;

public class TaskVM
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public IEnumerable<TaskFileVM>? Files { get; set; }
    public required UserVM User { get; set; }
}