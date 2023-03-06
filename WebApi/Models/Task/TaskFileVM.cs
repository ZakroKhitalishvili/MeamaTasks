
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace Meama_Tasks.Models.Task;

public class TaskFileVM
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
}