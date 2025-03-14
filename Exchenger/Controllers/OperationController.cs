using Exchenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exchenger.Controllers
{
    public class OperationController : Controller
    {
        private readonly ExchengerDbContext db;

        public OperationController(ExchengerDbContext context)
        {
            db = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ShowTable()
        {
            var operations = db.Operations
                   .Include(o => o.Customer)
                   .Include(o => o.DepositedСurrency)
                   .Include(o => o.ReceivedCurrency)
                   .ToList();

            return View(operations);
        }
    }
}
