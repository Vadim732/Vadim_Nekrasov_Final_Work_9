namespace ElectronicWallet.Models;

public class Payment
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public DateTime DatePayment { get; set; }
    public int WalletId { get; set; }
    public Wallet Wallet { get; set; }
    public int ServiceProviderId { get; set; }
    public ServiceProvider ServiceProvider { get; set; }
}