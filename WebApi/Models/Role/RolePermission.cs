
namespace Meama_Tasks.Models.Role;


public abstract class RolePermissionVM
{
    public bool CreateTask { get; set; }
    public bool UpdateTask { get; set; }
    public bool DeleteTask { get; set; }
    public bool ViewTasks { get; set; }
}

