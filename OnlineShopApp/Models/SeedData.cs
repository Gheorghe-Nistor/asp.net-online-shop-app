using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShopApp.Data;
using System.Collections.Generic;

namespace OnlineShopApp.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Verificăm dacă în baza de date există cel puțin un rol însemnând că a fost rulat codul
                // De aceea facem return pentru a nu insera rolurile încă o dată
                // Acestă metodă trebuie să se execute o singură data
                if (context.Roles.Any())
                {
                    return; // baza de date conține deja roluri
                }
                // CREAREA ROLURILOR IN BD
                // dacă nu conține roluri, acestea se vor crea
                context.Roles.AddRange(
                    new IdentityRole { Id = "2c5e174e-3b0e-446f-86af483d56fd7210", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                    new IdentityRole { Id = "2c5e174e-3b0e-446f-86af483d56fd7211", Name = "Collaborator", NormalizedName = "Collaborator".ToUpper() },
                    new IdentityRole { Id = "2c5e174e-3b0e-446f-86af483d56fd7212", Name = "User", NormalizedName = "User".ToUpper() }
                );
               
                // o nouă instanță pe care o vom utiliza pentru crearea parolelor utilizatorilor (parolele sunt de tip hash)
                var hasher = new PasswordHasher<ApplicationUser>();
                // CREAREA USERILOR IN BD
                // Se creează cate un user pentru fiecare rol
                context.Users.AddRange(
                    new ApplicationUser
                    {
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb0",
                        // primary key
                        UserName = "admin@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN@TEST.COM",
                        PasswordHash = hasher.HashPassword(null,"Admin1!")
                   },
                   new ApplicationUser
                   {
                       Id = "8e445865-a24d-4543-a6c6-9443d048cdb1",
                       // primary key
                       UserName = "collaborator@test.com",
                       EmailConfirmed = true,
                       NormalizedEmail = "COLLABORATOR@TEST.COM",
                       Email = "collaborator@test.com",
                       NormalizedUserName = "COLLABORATOR@TEST.COM",
                       PasswordHash = hasher.HashPassword(null,"Collaborator1!")
                   },
                    new ApplicationUser
                    {
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb2",
                        // primary key
                        UserName = "user@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER@TEST.COM",
                        PasswordHash = hasher.HashPassword(null,"User1!")
                    }
                );
                // ASOCIEREA USER-ROLE
                context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "2c5e174e-3b0e-446f-86af483d56fd7210",
                        UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
                    },
                    new IdentityUserRole<string>
                    {
                       RoleId = "2c5e174e-3b0e-446f-86af483d56fd7211",
                       UserId = "8e445865-a24d-4543-a6c6-9443d048cdb1"
                    },
                    new IdentityUserRole<string>
                    {
                       RoleId = "2c5e174e-3b0e-446f-86af483d56fd7212",
                       UserId = "8e445865-a24d-4543-a6c6-9443d048cdb2"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
