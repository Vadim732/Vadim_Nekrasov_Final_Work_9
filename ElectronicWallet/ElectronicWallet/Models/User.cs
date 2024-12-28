using Microsoft.AspNetCore.Identity;

namespace ElectronicWallet.Models;

public class User : IdentityUser<int>
{
    public string Avatar { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int UniqueNumber { get; set; }
    
    public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}