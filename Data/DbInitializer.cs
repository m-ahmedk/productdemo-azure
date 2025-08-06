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
                var (hash, salt) = HashHelper.HashPassword("ahmed123#");

                var user = new AppUser {
                    Email = "mahmedvilla@gmail.com",
                    PasswordHash = hash,
                    PasswordStamp = salt,
                    UserRoles = new List<UserRole>()
                };

                // Assign role to user
                Role? adminRole = context.Roles.FirstOrDefault(x => x.Name == "Admin");
                if (adminRole != null) {
                    user.UserRoles.Add(new UserRole { Role = adminRole });
                }

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
        }
    }
}
