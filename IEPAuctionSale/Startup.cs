using IEPAuctionSale.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IEPAuctionSale.Startup))]
namespace IEPAuctionSale
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CeateAdmin();
            app.MapSignalR();
        }

        private void CeateAdmin()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "stevan.milic@admin.com";
                user.Name = "AdminStevan";
                user.Surname = "AdminMilic";
                user.Email = "stevan.milic@admin.com";

                string pass = "Stevan123!";

                var chkUser = userManager.Create(user, pass);

                if (chkUser.Succeeded)
                {
                    var result1 = userManager.AddToRole(user.Id, "Admin");
                }
            }
        }
    }
}
