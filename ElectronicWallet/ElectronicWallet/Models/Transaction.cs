namespace ElectronicWallet.Models;

public class Transaction
{
    public int Id { get; set; }
    public DateTime DateTransaction { get; set; }
    public int Amount { get; set; }
    public TransactionType Type { get; set; } 
    
    public int WalletId { get; set; }
    public Wallet Wallet { get; set; }
    
    public int? CounterpartyId { get; set; }
    public User Counterparty { get; set; }
}