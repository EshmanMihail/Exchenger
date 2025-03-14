using Exchenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exchenger.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ExchengerDbContext db;

        public CustomerController(ExchengerDbContext context)
        {
            db = context;
        }

        public IActionResult ShowTable()
        {
            var customers = db.Customers.Include(c => c.Role);
            return View(customers.ToList());
        }

        public IActionResult Filter()
        {
            string? searchName = HttpContext.Request.Form["SearchName"];
            string? searchLastName = HttpContext.Request.Form["SearchSecondName"];
            string? searchPhone = HttpContext.Request.Form["SearchPhone"];

            var customers = db.Customers.Include(c => c.Role).AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                customers = customers.Where(c => c.FirstName.Contains(searchName));
            }
            if (!string.IsNullOrEmpty(searchLastName))
            {
                customers = customers.Where(c => c.LastName.Contains(searchLastName));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                customers = customers.Where(c => c.PhoneNumber.Contains(searchPhone));
            }

            return View("ShowTable", customers.ToList());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(db.CustomerRoles, "RoleId", "RoleName");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create([Bind("FirstName,LastName,RoleId,Email,PhoneNumber")] Customer customer)
        {
            customer.CreatedDate = DateTime.Today;

            ViewBag.Customer = customer;

            db.Add(customer);
            db.SaveChanges();
            ViewBag.Resalt = 1;

            ViewData["RoleId"] = new SelectList(db.CustomerRoles, "RoleId", "RoleName", customer.RoleId);
            return View(customer);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Update(int? id)
        {
            var customer = db.Customers.Find(id);

            ViewData["RoleId"] = new SelectList(db.CustomerRoles, "RoleId", "RoleName", customer.RoleId);
            return View(customer);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(int id, [Bind("Id,FirstName,LastName,RoleId,Email,PhoneNumber,CreatedDate")] Customer customer)
        {
            db.Update(customer);
            db.SaveChanges();
            ViewBag.Resalt = 1;
            ViewData["RoleId"] = new SelectList(db.CustomerRoles, "RoleId", "RoleName", customer.RoleId);
            return View(customer);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            var customer = db.Customers.Where(c => c.Id == id).FirstOrDefault();
            ViewBag.Rolee = db.CustomerRoles.Where(c => c.RoleId == customer.RoleId).FirstOrDefault();
            return View(customer);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = db.Customers.Find(id);
            if (customer != null)
            {
                db.Customers.Remove(customer);
                db.SaveChanges();
                ViewBag.Resalt = 1;
            }

            List<Customer> list = db.Customers.Include(c => c.Role).ToList();
            return View("ShowTable", list);
        }
    }
}
