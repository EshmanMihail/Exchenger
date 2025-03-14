using Exchenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exchenger.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ExchengerDbContext db;

        public CurrencyController(ExchengerDbContext context)
        {
            db = context;
        }

        public IActionResult ShowTable()
        {
            List<Currency> list = db.Currencies.ToList();

            return View(list);
        }

        public IActionResult FilterByNameAndAbbr()
        {
            string? searchName = HttpContext.Request.Form["SearchName"];
            string? searchAbbr = HttpContext.Request.Form["SearchAbbr"];

            var currencies = db.Currencies.AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                currencies = currencies.Where(c => c.CurName.Contains(searchName));
            }
            if (!string.IsNullOrEmpty(searchAbbr))
            {
                currencies = currencies.Where(c => c.CurAbbreviation.Contains(searchAbbr));
            }

            return View("ShowTable", currencies.ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            var currency = db.Currencies.Find(id);
            return View(currency);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(int id, [Bind("CurId,CurName,CurAbbreviation,CurScale,CurPeriodicity,CurOfficialRate")] Currency currency)
        {
            if (id == currency.CurId && ModelState.IsValid)
            {
                db.Update(currency);
                db.SaveChanges();
                ViewBag.Resalt = 1;
            }
            else
            {
                ViewBag.Resalt = 0;
            }

            return View(currency);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create([Bind("CurId,CurName,CurAbbreviation,CurScale,CurPeriodicity,CurOfficialRate")] Currency currency)
        {
            if (ModelState.IsValid)
            {
                db.Add(currency);
                db.SaveChanges();
                ViewBag.Resalt = 1;
            }
            else
            {
                ViewBag.Resalt = 0;
            }

            return View(currency);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            var currency = db.Currencies.Find(id);
            if (currency == null)
            {
                ViewBag.CurrencyNull = "Данная валюта отсутствует в базе";
            }

            return View(currency);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var currency = db.Currencies.Find(id);
            if (currency != null)
            {
                db.Currencies.Remove(currency);
                db.SaveChanges();
                ViewBag.Resalt = "Данные удалены.";
            }

            List<Currency> list = db.Currencies.ToList();
            return View("ShowTable", list);
        }
    }
}
