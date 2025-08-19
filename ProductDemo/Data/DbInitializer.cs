using ProductDemo.Helpers;
using ProductDemo.Models;

namespace ProductDemo.Data
{
    public static class DbInitializer
    {
        public async static Task Seed(IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await SeedRoles(context);
            await SeedUsers(context);
        }

        public async static Task SeedRoles(AppDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roles = new List<Role>()
                {
                    new Role { Name = "Admin" },
                    new Role {Name = "User"}
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }
        }

        public async static Task SeedUsers(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                // ---- Admin user ----
                var (adminHash, adminSalt) = HashHelper.HashPassword("ahmed123#");

                var adminUser = new AppUser
                {
                    Email = "mahmedvilla@gmail.com",
                    PasswordHash = adminHash,
                    PasswordStamp = adminSalt,
                    UserRoles = new List<UserRole>()
                };

                Role? adminRole = context.Roles.FirstOrDefault(x => x.Name == "Admin");
                if (adminRole != null)
                {
                    adminUser.UserRoles.Add(new UserRole { Role = adminRole });
                }

                await context.Users.AddAsync(adminUser);

                // ---- Normal user ----
                var (userHash, userSalt) = HashHelper.HashPassword("user123#");

                var normalUser = new AppUser
                {
                    Email = "normaluser@test.com",
                    PasswordHash = userHash,
                    PasswordStamp = userSalt,
                    UserRoles = new List<UserRole>()
                };

                Role? userRole = context.Roles.FirstOrDefault(x => x.Name == "User");
                if (userRole != null)
                {
                    normalUser.UserRoles.Add(new UserRole { Role = userRole });
                }

                await context.Users.AddAsync(normalUser);

                // Save both users
                await context.SaveChangesAsync();
            }
        }

    }
}
