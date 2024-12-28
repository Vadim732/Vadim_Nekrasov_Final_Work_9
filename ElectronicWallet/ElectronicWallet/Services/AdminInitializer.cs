using ElectronicWallet.Models;
using Microsoft.AspNetCore.Identity;

namespace ElectronicWallet.Services;

public class AdminInitializer
{
    public static async Task SeedRolesAndAdmin(RoleManager<IdentityRole<int>> _roleManager, UserManager<User> _userManager, WalletContext _context)
    {
        string adminEmail = "admin@admin.admin";
        string adminUserName = "AdminAdminovich";
        string adminPassword = "Admin123$QwE";
        string adminPhoneNumber = "996550000001";
        int adminUniqueNumber = 123456;
        string adminAvatar = "https://i.pinimg.com/736x/e3/e0/9b/e3e09bf6548359de5812e78a05adf964.jpg";
        DateTime adminDateOfBirth = new DateTime(2002, 2, 22).ToUniversalTime();
        
        var roles = new[] { "admin", "user" };
        
        foreach (var role in roles)
        {
            if (await _roleManager.FindByNameAsync(role) == null)
                await _roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        if (await _userManager.FindByEmailAsync(adminEmail) == null)
        {
            User admin = new User
            {
                Email = adminEmail,
                UserName = adminUserName,
                Avatar = adminAvatar,
                DateOfBirth = adminDateOfBirth,
                UniqueNumber = adminUniqueNumber,
                PhoneNumber = adminPhoneNumber
            };
            
            IdentityResult result = await _userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, "admin");
                Wallet adminWallet = new Wallet
                {
                    Balance = 100,
                    UserId = admin.Id
                };
                
                _context.Wallets.Add(adminWallet);
                await _context.SaveChangesAsync();
            }
        }
    }
}