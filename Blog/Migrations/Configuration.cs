namespace Blog.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Blog.Models;
    using Blog.Models.Domain;



    internal sealed class Configuration : DbMigrationsConfiguration<Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Blog.Models.ApplicationDbContext";
        }

        protected override void Seed(Blog.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            //RoleManager, used to manage roles
            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

            //UserManager, used to manage users
            var userManager =
                new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(context));

            //Adding admin role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                roleManager.Create(adminRole);
            }

            //Creating the adminuser
            ApplicationUser adminUser;

            if (!context.Users.Any(
                p => p.UserName == "admin@blog.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@blog.com";
                adminUser.Email = "admin@blog.com";

                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context
                    .Users
                    .First(p => p.UserName == "admin@blog.com");
            }

            //Make sure the user is on the admin role
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            //Adding moderator role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "Moderator"))
            {
                var moderatorRole = new IdentityRole("Moderator");
                roleManager.Create(moderatorRole);
            }

            //Creating the adminuser
            ApplicationUser moderatorUser;

            if (!context.Users.Any(
                p => p.UserName == "moderator@blog.com"))
            {
                moderatorUser = new ApplicationUser();
                moderatorUser.UserName = "moderator@blog.com";
                moderatorUser.Email = "moderator@blog.com";

                userManager.Create(moderatorUser, "Password-1");
            }
            else
            {
                moderatorUser = context
                    .Users
                    .First(p => p.UserName == "moderator@blog.com");
            }

            //Make sure the user is on the admin role
            if (!userManager.IsInRole(moderatorUser.Id, "Moderator"))
            {
                userManager.AddToRole(moderatorUser.Id, "Moderator");
            }


        }

    }
}
