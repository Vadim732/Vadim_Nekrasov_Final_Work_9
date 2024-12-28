namespace ElectronicWallet.Models;

public class ServiceProvider
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AccountDetails { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}