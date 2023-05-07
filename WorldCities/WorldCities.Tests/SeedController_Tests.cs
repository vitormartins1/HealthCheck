using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using WorldCities.Controllers;
using WorldCities.Data;
using WorldCities.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCities.Tests
{
    public class SeedController_Tests
    {
        [Fact]
        public async void CreateDefaultUsers()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "WorldCities")
                .Options;

            IOptions<OperationalStoreOptions> storeOptions = Options.Create(new OperationalStoreOptions());

            IWebHostEnvironment mockEnv = new Mock<IWebHostEnvironment>().Object;

            ApplicationUser user_Admin = null;
            ApplicationUser user_User = null;
            ApplicationUser user_NotExisting = null;

            using (ApplicationDbContext context = new(options, storeOptions))
            {
                RoleManager<IdentityRole> roleManager = IdentityHelper
                    .GetRoleManager(new RoleStore<IdentityRole>(context));

                UserManager<ApplicationUser> userManager = IdentityHelper
                    .GetUserManager(new UserStore<ApplicationUser>(context));

                SeedController controller = new SeedController(
                    context,
                    roleManager,
                    userManager,
                    mockEnv);

                await controller.CreateDefaultUsers();

                user_Admin = await userManager.FindByEmailAsync("admin@email.com");
                user_User = await userManager.FindByEmailAsync("user@email.com");
                user_NotExisting = await userManager.FindByEmailAsync("notexisting@email.com");

                Assert.NotNull(user_Admin);
                Assert.NotNull(user_User);
                Assert.Null(user_NotExisting);
            };
        }
    }
}
