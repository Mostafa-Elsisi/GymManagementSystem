using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagement.DAL.Data.DataSeeding
{
    public static class IdentityDataSeeding
    {

        public static async Task SeedIdentityDataAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger logger, CancellationToken ct = default)
        {
            try
            {

                bool hasUsers = await userManager.Users.AnyAsync(ct);
                bool hasRoles = await roleManager.Roles.AnyAsync(ct);

                if (hasUsers && hasRoles) return;

                var roles = new List<IdentityRole>()
            {
                new IdentityRole ("SuperAdmin"),
                new IdentityRole ("Admin")
            };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name!))
                    {
                        var roleResult = await roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            logger.LogError($"Failed To Create Role{role.Name} : {string.Join(" ; ", roleResult.Errors.Select(e => e.Description))}");
                        }

                    }
                }

                if (!hasUsers)
                {
                    var MainAdmin = new ApplicationUser()
                    {

                        FirstName = "Mostafa",
                        LastName = "Elsisi",
                        UserName = "MostafaElsisi",
                        Email = "mostafaelsisi@gmail.com",
                        PhoneNumber = "01033810545"
                    };
                    await userManager.CreateAsync(MainAdmin, "P@ssW0rd");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");



                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Mohamed",
                        LastName = "Elsisi",
                        UserName = "MohammedElsisi",
                        Email = "mohamedelsisi@gmail.com",
                        PhoneNumber = "01108058950"
                    };
                    await userManager.CreateAsync(Admin, "P@ssW0rd");
                    await userManager.AddToRoleAsync(Admin, "Admin");


                    logger.LogInformation("Identity Data Seeded");
                }

                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Identity Seeding Failed"); return;
            }
        }
    }
}
