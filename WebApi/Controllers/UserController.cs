using Infrastructure.Database;
using Infrastructure.Entities;
using Meama_Tasks.Attributes;
using Meama_Tasks.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Meama_Tasks.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[AuthorizePermission(RequireRootRole = true)]
public class UserController : ControllerBase
{
    private readonly TaskDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public UserController(TaskDbContext dbContext,
        UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<ActionResult<IList<UserVM>>> GetUsers()
    {
        var appUsers = await _dbContext.Users.ToListAsync();

        return Ok(appUsers.Select(appUser =>
        new UserVM
        {
            Email = appUser.Email,
            Id = appUser.Id,
            Name = appUser.UserName
        }));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IList<UserVM>>> GetUser(string id)
    {
        var appUser = await _userManager.FindByIdAsync(id);

        if (appUser is null)
        {
            return BadRequest("User not found");
        }

        return Ok(new UserVM
        {
            Email = appUser.Email,
            Id = appUser.Id,
            Name = appUser.UserName
        });
    }

    [HttpPost]
    public async Task<ActionResult<UserVM>> PostUser(UserCreateVM userCreate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var appRole = await _roleManager.FindByNameAsync(userCreate.Role);

        if (appRole is null)
        {
            return NotFound("Role not found");
        }

        var user = new AppUser() { UserName = userCreate.Name, Email = userCreate.Email };

        var result = await _userManager.CreateAsync(
            user,
            userCreate.Password
        );

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, userCreate.Role);

        if (!roleResult.Succeeded)
        {
            return BadRequest(roleResult.Errors);
        }

        var appUser = await _userManager.FindByEmailAsync(userCreate.Email);

        return Created("", new UserVM { Email = userCreate.Email, Name = userCreate.Name, Id = appUser!.Id });

    }

    [HttpPut]
    public async Task<ActionResult<UserVM>> PutUser(UserUpdateVM userUpdate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var appUser = await _userManager.FindByIdAsync(
           userUpdate.Id
        );

        if (appUser is null)
        {
            return NotFound("User not found");
        }

        var appRole = await _roleManager.FindByNameAsync(userUpdate.Role);

        if (appRole is null)
        {
            return NotFound("Role not found");
        }

        appUser.Email = userUpdate.Email;
        appUser.UserName = userUpdate.Name;

        var result = await _userManager.UpdateAsync(appUser);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        if (!await _userManager.IsInRoleAsync(appUser, appRole.Name!))
        {
            var role = (await _userManager.GetRolesAsync(appUser)).FirstOrDefault();
            if (role is not null)
            {
                await _userManager.RemoveFromRoleAsync(appUser, role);
            }
            var roleResult = await _userManager.AddToRoleAsync(appUser, appRole.Name!);

            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }
        }

        if (userUpdate.NewPassword is not null)
        {
            var passwordUpdateResult = await _userManager.ChangePasswordAsync(appUser, userUpdate.Password, userUpdate.NewPassword);

            if (!passwordUpdateResult.Succeeded)
            {
                return BadRequest(passwordUpdateResult.Errors);
            }

        }

        return Ok(new UserVM { Email = appUser.Email, Id = appUser.Id, Name = appUser.UserName});

    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        var appUser = await _userManager.FindByIdAsync(id);

        if (appUser is null)
        {
            return BadRequest("User not found");
        }

        var deleteResult = await _userManager.DeleteAsync(appUser);

        if (!deleteResult.Succeeded)
        {
            return BadRequest(deleteResult.Errors);
        }

        return Ok();
    }

}