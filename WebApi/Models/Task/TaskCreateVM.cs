
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Meama_Tasks.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace Meama_Tasks.Models.Task;

public class TaskCreateVM
{
    public required string Title { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public IFormFileCollection? Files { get; set; }
}