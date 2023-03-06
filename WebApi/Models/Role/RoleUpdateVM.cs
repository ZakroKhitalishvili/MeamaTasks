
using System.ComponentModel.DataAnnotations;

namespace Meama_Tasks.Models.Role;


public class RoleUpdateVM: RolePermissionVM
{
    [Required]
    public required string Name { get; set; }
}

