using Microsoft.AspNetCore.Mvc;

namespace ElectronicWallet.Controllers;

public class TransactionController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}