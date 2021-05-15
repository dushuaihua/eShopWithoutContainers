using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Data
{
    public class ApplicationDbContextSeed
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();
        public async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(GetDefaultUser());
                await context.SaveChangesAsync();
            }
        }
        private IEnumerable<ApplicationUser> GetDefaultUser()
        {
            var user =
            new ApplicationUser()
            {
                CardHolderName = "DemoUser",
                CardNumber = "4012888888881881",
                CardType = 1,
                City = "Redmond",
                Country = "U.S.",
                Email = "demouser@microsoft.com",
                Expiration = "12/21",
                Id = Guid.NewGuid().ToString(),
                LastName = "DemoLastName",
                Name = "DemoUser",
                PhoneNumber = "1234567890",
                UserName = "demouser@microsoft.com",
                ZipCode = "98052",
                State = "WA",
                Street = "15703 NE 61st Ct",
                SecurityNumber = "535",
                NormalizedEmail = "DEMOUSER@MICROSOFT.COM",
                NormalizedUserName = "DEMOUSER@MICROSOFT.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, "11111111");

            return new List<ApplicationUser>()
            {
                user
            };
        }
    }
}
