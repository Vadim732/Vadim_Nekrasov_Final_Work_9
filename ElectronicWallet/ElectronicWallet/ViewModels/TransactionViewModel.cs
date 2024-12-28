namespace ElectronicWallet.ViewModels;

public class TransactionViewModel
{
    public DateTime DateTransaction { get; set; }
    public string Type { get; set; }
    public string Counterparty { get; set; }
    public int Amount { get; set; }
}