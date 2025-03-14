using Exchenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exchenger.Controllers
{
    public class CurrencyAmountController : Controller
    {
        private readonly ExchengerDbContext db;

        public CurrencyAmountController(ExchengerDbContext context)
        {
            db = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ShowTable()
        {
            List<CurrencyAmount> list = db.CurrencyAmounts.Include(ca => ca.Currency).ToList();

            return View(list);
        }

        public IActionResult FilterByName()
        {
            string? searchName = HttpContext.Request.Form["SearchName"];

            var currencies = db.CurrencyAmounts.Include(c => c.Currency).AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                currencies = currencies.Where(c => c.Currency.CurName.Contains(searchName));
            }

            return View("ShowTable", currencies.ToList());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Fill(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currencyAmount = db.CurrencyAmounts.Include(ca => ca.Currency).FirstOrDefault(ca => ca.Id == id);

            if (currencyAmount == null)
            {
                return NotFound();
            }

            return View(currencyAmount);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Fill(int Id, int addAmount)
        {
            var currencyAmount = db.CurrencyAmounts.Include(ca => ca.Currency).FirstOrDefault(ca => ca.Id == Id);

            if (currencyAmount == null)
            {
                return NotFound();
            }

            currencyAmount.Amount = (currencyAmount.Amount ?? 0) + addAmount;

            db.Update(currencyAmount);
            db.SaveChanges();

            ViewBag.Result = "Сумма успешно пополнена!";
            return View(currencyAmount);
        }
    }
}
