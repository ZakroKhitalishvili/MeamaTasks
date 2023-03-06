
using System.ComponentModel.DataAnnotations;

namespace Meama_Tasks.Models.Role;


public class RoleVM: RolePermissionVM
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public bool IsRootRole { get; set; }
}

