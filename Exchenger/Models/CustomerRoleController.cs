using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exchenger.Models
{
    public class CustomerRoleController : Controller
    {
        private readonly ExchengerDbContext db;

        public CustomerRoleController(ExchengerDbContext context)
        {
            db = context;
        }
        public IActionResult ShowTable()
        {
            var list = db.CustomerRoles.ToList();
            return View(list);
        }

        public IActionResult FilterByName()
        {
            string? searchName = HttpContext.Request.Form["SearchName"];

            var currencies = db.CustomerRoles.AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                currencies = currencies.Where(c => c.RoleName.Contains(searchName));
            }

            return View("ShowTable", currencies.ToList());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create([Bind("RoleName")] CustomerRole role)
        {
            if (ModelState.IsValid)
            {
                db.Add(role);
                db.SaveChanges();
                ViewBag.Resalt = 1;
            }
            else
            {
                ViewBag.Resalt = 0;
            }

            return View(role);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            var currency = db.CustomerRoles.Find(id);
            return View(currency);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(int id, [Bind("RoleName")] CustomerRole role)
        {
            if (ModelState.IsValid)
            {
                var existingRole = db.CustomerRoles.Find(id);
                if (existingRole != null)
                {
                    existingRole.RoleName = role.RoleName;
                    db.SaveChanges();
                    ViewBag.Resalt = 1;
                }
                else ViewBag.Resalt = 0;
            }
            else
            {
                ViewBag.Resalt = 0;
            }

            return View(role);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            var role = db.CustomerRoles.Find(id);
            if (role == null)
            {
                ViewBag.CurrencyNull = "Данная валюта отсутствует в базе";
            }

            return View(role);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var role = db.CustomerRoles.Find(id);
            if (role != null)
            {
                db.CustomerRoles.Remove(role);
                db.SaveChanges();
                ViewBag.Resalt = "Данные удалены.";
            }

            List<CustomerRole> list = db.CustomerRoles.ToList();
            return View("ShowTable", list);
        }
    }
}
