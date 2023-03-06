using Infrastructure.Database;
using Infrastructure.Entities;
using Meama_Tasks.Attributes;
using Meama_Tasks.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Meama_Tasks.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[AuthorizePermission(RequireRootRole = true)]
public class RoleController : ControllerBase
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly TaskDbContext _dbContext;

    public RoleController(RoleManager<AppRole> roleManager, TaskDbContext dbContext)
    {
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IList<RoleVM>>> GetRoles()
    {
        var appRoles = await _dbContext.Roles.ToListAsync();

        return Ok(appRoles.Select(MapRole));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IList<RoleVM>>> GetRole(string id)
    {
        var appRole = await _roleManager.FindByIdAsync(id);

        if (appRole is null)
        {
            return BadRequest("Role not found");
        }

        return Ok(MapRole(appRole));
    }


    [HttpPost]
    public async Task<ActionResult> PostRole(RoleCreateVM roleCreate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var appRole = new AppRole()
        {
            Name = roleCreate.Name,
            IsRootRole = false,
            Permission = MapRolePermission(roleCreate)
        };

        var result = await _roleManager.CreateAsync(appRole);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Created("", MapRole(appRole));
    }

    [HttpPut]
    public async Task<ActionResult> PutRole(RoleUpdateVM roleUpdate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var appRole = await _roleManager.FindByNameAsync(roleUpdate.Name);

        if (appRole is null)
        {
            return BadRequest("Role not found");
        }

        if (appRole.IsRootRole)
        {
            return Forbid();
        }
        appRole.Permission = MapRolePermission(roleUpdate);

        var result = await _roleManager.UpdateAsync(appRole);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(MapRole(appRole));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        var appRole = await _roleManager.FindByIdAsync(id);

        if (appRole is null)
        {
            return BadRequest("Role not found");
        }

        if (appRole.IsRootRole)
        {
            return Forbid();
        }

        var deleteResult = await _roleManager.DeleteAsync(appRole);

        if (!deleteResult.Succeeded)
        {
            return BadRequest(deleteResult.Errors);
        }

        return Ok();
    }


    private RoleVM MapRole(AppRole appRole)
    {
        return new RoleVM()
        {
            Id = appRole.Id,
            Name = appRole.Name,
            IsRootRole = appRole.IsRootRole,
            CreateTask = appRole.Permission.Contains(Permission.CreateTask),
            ViewTasks = appRole.Permission.Contains(Permission.ViewTasks),
            DeleteTask = appRole.Permission.Contains(Permission.DeleteTask),
            UpdateTask = appRole.Permission.Contains(Permission.UpdateTask),
        };
    }

    private Permission MapRolePermission(RolePermissionVM roleVM)
    {
        Permission permission = Permission.None;

        if (roleVM.CreateTask) permission |= Permission.CreateTask;
        if (roleVM.UpdateTask) permission |= Permission.UpdateTask;
        if (roleVM.DeleteTask) permission |= Permission.DeleteTask;
        if (roleVM.ViewTasks) permission |= Permission.ViewTasks;

        return permission;
    }

}