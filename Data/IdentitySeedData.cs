using Assignment1.Data;
using Microsoft.AspNetCore.Identity;

public class IdentitySeedData {
    public static async Task Initialize(ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager) {
        context.Database.EnsureCreated();

        string asdminRole = "admin";
        string memberRole = "contributor";
        string password4all = "P@$$w0rd";

        if (await roleManager.FindByNameAsync(asdminRole) == null) {
            await roleManager.CreateAsync(new IdentityRole(asdminRole));
        }

        if (await roleManager.FindByNameAsync(memberRole) == null) {
            await roleManager.CreateAsync(new IdentityRole(memberRole));
        }

        if (await userManager.FindByNameAsync("a@a.a") == null){
            var user = new IdentityUser {
                UserName = "a@a.a",
                Email = "a@a.a"
            };

            var result = await userManager.CreateAsync(user);
            if (result.Succeeded) {
                await userManager.AddPasswordAsync(user, password4all);
                await userManager.AddToRoleAsync(user, asdminRole);
            }
        }

        if (await userManager.FindByNameAsync("c@c.c") == null) {
            var user = new IdentityUser {
                UserName = "c@c.c",
                Email = "c@c.c"
            };

            var result = await userManager.CreateAsync(user);
            if (result.Succeeded) {
                await userManager.AddPasswordAsync(user, password4all);
                await userManager.AddToRoleAsync(user, memberRole);
            }
        }

        
    }
}
