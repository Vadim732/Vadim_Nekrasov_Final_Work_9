using ElectronicWallet.Models;
using ElectronicWallet.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace ElectronicWallet.Controllers;

public class TransactionController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly WalletContext _context;

    public TransactionController(UserManager<User> userManager, SignInManager<User> signInManager,
        WalletContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            if (wallet != null)
            {
                ViewBag.Balance = wallet.Balance;
            }
        }

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> TopUpBalance(int UniqueNumber, int Amount)
    {
        if (Amount <= 0)
        {
            return Json(new { success = false, errorMessage = "Сумма пополнения должна быть больше нуля." });
        }

        var user = await _context.Users.Include(u => u.Wallets)
            .FirstOrDefaultAsync(u => u.UniqueNumber == UniqueNumber);

        if (user == null)
        {
            return Json(new { success = false, errorMessage = "Указанный номер счёта не существует." });
        }

        var wallet = user.Wallets.FirstOrDefault();
        wallet.Balance += Amount;

        var transaction = new Transaction
        {
            DateTransaction = DateTime.UtcNow,
            Amount = Amount,
            Type = TransactionType.Refill,
            WalletId = wallet.Id
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Json(new { success = true, newBalance = wallet.Balance });
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> TransferOfFunds(int UniqueNumber, int Amount)
    {
        if (Amount <= 0)
        {
            return Json(new { success = false, errorMessage = "Сумма перевода должна быть больше нуля." });
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Json(new { success = false, errorMessage = "Вы должны быть авторизованы для выполнения перевода." });
        }

        var senderWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == currentUser.Id);
        if (senderWallet == null || senderWallet.Balance < Amount)
        {
            return Json(new { success = false, errorMessage = "Недостаточно средств для перевода." });
        }

        var recipientUser = await _context.Users.Include(u => u.Wallets)
            .FirstOrDefaultAsync(u => u.UniqueNumber == UniqueNumber);
        if (recipientUser == null)
        {
            return Json(new { success = false, errorMessage = "Пользователь с указанным номером не найден." });
        }

        var recipientWallet = recipientUser.Wallets.FirstOrDefault();
        senderWallet.Balance -= Amount;
        recipientWallet.Balance += Amount;

        var senderTransaction = new Transaction
        {
            DateTransaction = DateTime.UtcNow,
            Amount = -Amount,
            Type = TransactionType.Transfer,
            WalletId = senderWallet.Id,
            CounterpartyId = recipientUser.Id
        };

        var recipientTransaction = new Transaction
        {
            DateTransaction = DateTime.UtcNow,
            Amount = Amount,
            Type = TransactionType.Transfer,
            WalletId = recipientWallet.Id,
            CounterpartyId = currentUser.Id
        };

        _context.Transactions.AddRange(senderTransaction, recipientTransaction);
        await _context.SaveChangesAsync();

        return Json(new { success = true, newBalance = senderWallet.Balance });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ListOfTransactions(DateTime? dateFrom, DateTime? dateTo)
    {
        var user = await _userManager.GetUserAsync(User);
        if (dateFrom.HasValue)
        {
            dateFrom = DateTime.SpecifyKind(dateFrom.Value, DateTimeKind.Utc);
        }
        if (dateTo.HasValue)
        {
            dateTo = DateTime.SpecifyKind(dateTo.Value, DateTimeKind.Utc);
        }

        var query = _context.Transactions.Include(t => t.Counterparty).Where(t => t.Wallet.UserId == user.Id);
        if (dateFrom.HasValue)
        {
            query = query.Where(t => t.DateTransaction >= dateFrom.Value);
        }
        if (dateTo.HasValue)
        {
            query = query.Where(t => t.DateTransaction <= dateTo.Value);
        }

        var transactions = await query.ToListAsync();
        var transactionModels = transactions.Select(t => new TransactionViewModel
        {
            DateTransaction = t.DateTransaction,
            Type = t.Type.ToString(),
            Counterparty = t.Type == TransactionType.Refill 
                ? "Пополнение с терминала" 
                : t.Counterparty?.UniqueNumber.ToString() ?? "Неизвестно",
            Amount = t.Amount
        }).ToList();

        return View(transactionModels);
    }
}