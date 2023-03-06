using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class AppRole : IdentityRole
{
    public required bool IsRootRole { get; set; }
    public required Permission Permission { get; set; }
}


[Flags]
public enum Permission
{
    None = 0,
    CreateTask = 1,
    UpdateTask = 2,
    DeleteTask = 4,
    ViewTasks = 8
}

public static class PermissionExtensions
{

    public static bool Contains(this Permission permission, Permission checkPermission)
    {
        return (permission & checkPermission) == checkPermission;
    }
}