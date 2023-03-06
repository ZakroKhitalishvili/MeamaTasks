
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

public static class DataSeeder
{
    public static async Task Seed(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        var adminRole = new AppRole
        {
            Name = "Admin",
            IsRootRole = true,
            Permission = Permission.None
        };
        await roleManager.CreateAsync(adminRole);

        var adminUser = new AppUser
        {
            UserName = "Admin",
            Email = "admin@meama.ge"
        };

        var result = await userManager.CreateAsync(adminUser,"admin");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole.Name);
        }

    }
}