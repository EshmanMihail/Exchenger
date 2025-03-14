using System.Diagnostics;
using Exchenger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Exchenger.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ExchengerDbContext db;

        public HomeController(ILogger<HomeController> logger, ExchengerDbContext db)
        {
            _logger = logger;
            this.db = db;
        }

        public IActionResult Index()
        {
            List<Currency> currencies = db.Currencies.Where(c => c.CurOfficialRate != null).ToList();

            ViewData["Currencies"] = new SelectList(currencies, "CurId", "CurName");

            ViewBag.TodayDate = DateTime.Now.ToString().Split(' ')[0];

            ViewBag.Customers = db.Customers.ToList();

            return View(currencies);
        }


        [HttpPost]
        public IActionResult Exchange(int InputCurrencyId, int OutputCurrencyId, int InputAmount)
        {
            Currency inputCurrency = db.Currencies.Find(InputCurrencyId);
            Currency outputCurrency = db.Currencies.Find(OutputCurrencyId);

            int? outputAmount = GetOutPutAmount(inputCurrency, outputCurrency, InputAmount);

            ViewBag.AmountOfCurrencyNotEnough = "ŒÔÂ‡ˆËˇ ÔÓ¯Î‡ ÛÒÔÂ¯ÌÓ.";
            if (outputAmount == -1)
            {
                outputAmount = 0;
                ViewBag.AmountOfCurrencyNotEnough = "¬ Í‡ÁÌÂ ÌÂ ‰ÓÒÚ‡ÚÓ˜ÌÓ ‰ÂÌÂ„ ˜ÚÓ·˚ Ó·ÏÂÌˇÚ¸ Ì‡ ÚÂÍÛ˘Û˛ ÒÛÏÏÛ!";
            }
            else
            {
                CreateAndAddOperation(inputCurrency, outputCurrency, InputAmount, (int)outputAmount);
            }

            ViewBag.InputCurrency = inputCurrency.CurName;
            ViewBag.OutputCurrency = outputCurrency.CurName;
            ViewBag.InputAmount = InputAmount;
            ViewBag.OutputAmount = outputAmount;

            return View("ExchangeResult");
        }

        private int? GetOutPutAmount(Currency inputCurrency, Currency outputCurrency, int inputAmount)
        {
            int? outputAmount = 0;

            if (outputCurrency.CurAbbreviation == "BEL")
            {
                outputAmount = GetBelRubFromCurrency(inputCurrency, outputCurrency, inputAmount);
            }
            else if (inputCurrency.CurAbbreviation == "BEL")
            {
                outputAmount = GetCurrencyFromBelRub(inputCurrency, outputCurrency, inputAmount);
            }
            else
            {
                outputAmount = ExchangeCurrency(inputCurrency, outputCurrency, inputAmount);
            }

            return outputAmount;
        }

        private int? GetBelRubFromCurrency(Currency inputCurrency, Currency bel, int inputAmount)
        {
            int? outputBelRub = (int)((inputAmount / inputCurrency.CurScale) * inputCurrency.CurOfficialRate);

            CurrencyAmount amountOfBelRubObj = db.CurrencyAmounts.Where(c => c.CurrencyId == bel.CurId).FirstOrDefault();

            if (amountOfBelRubObj.Amount < outputBelRub) return -1;
            else
            {
                amountOfBelRubObj.Amount -= outputBelRub;
                db.Update(amountOfBelRubObj);

                CurrencyAmount amountOfInputCurrency = db.CurrencyAmounts.Where(c => c.CurrencyId == inputCurrency.CurId).FirstOrDefault();
                amountOfInputCurrency.Amount += inputAmount;
                db.Update(amountOfInputCurrency);

                db.SaveChanges();
            }

            return outputBelRub;
        }

        private int? GetCurrencyFromBelRub(Currency bel, Currency outputCurrency, int inputAmount)
        {
            double? outputAmount = (inputAmount / outputCurrency.CurOfficialRate) * outputCurrency.CurScale;

            CurrencyAmount amountOfCurrencyObj = db.CurrencyAmounts.Where(c => c.CurrencyId == outputCurrency.CurId).FirstOrDefault();

            if (amountOfCurrencyObj.Amount < outputAmount) return -1;
            else
            {
                amountOfCurrencyObj.Amount -= (int)outputAmount;
                db.Update(amountOfCurrencyObj);

                CurrencyAmount amountOfInputCurrency = db.CurrencyAmounts.Where(c => c.CurrencyId == bel.CurId).FirstOrDefault();
                amountOfInputCurrency.Amount += inputAmount;
                db.Update(amountOfInputCurrency);

                db.SaveChanges();
            }

            return (int)outputAmount;
        }

        private int? ExchangeCurrency(Currency inputCurrency, Currency outputCurrency, int inputAmount)
        {
            double? belRubAmount = (inputAmount / inputCurrency.CurScale) * inputCurrency.CurOfficialRate;

            double? outputAmount = (belRubAmount / outputCurrency.CurOfficialRate) * outputCurrency.CurScale;

            CurrencyAmount amountOfOutputCurrency = db.CurrencyAmounts.Where(c => c.CurrencyId == outputCurrency.CurId).FirstOrDefault();

            if (amountOfOutputCurrency.Amount < outputAmount) return -1;

            amountOfOutputCurrency.Amount -= (int)outputAmount;
            db.Update(amountOfOutputCurrency);

            CurrencyAmount amountOfInputCurrency = db.CurrencyAmounts.Where(c => c.CurrencyId == inputCurrency.CurId).FirstOrDefault();
            amountOfInputCurrency.Amount += inputAmount;
            db.Update(amountOfInputCurrency);

            db.SaveChanges();

            return (int?)outputAmount;
        }

        private void CreateAndAddOperation(Currency inputCurrency, Currency outputCurrency, int inputAmount, int outputAmount)
        {
            Operation operation = new Operation();
            operation.CustomerId = 8;
            operation.Deposited—urrencyId = inputCurrency.CurId;
            operation.ReceivedCurrencyId = outputCurrency.CurId;
            operation.AmountOfDeposited = inputAmount;
            operation.AmountOfReceived = outputAmount;
            operation.CreatedDate = DateTime.Today;
            operation.Deposited—urrency = inputCurrency;
            operation.ReceivedCurrency = outputCurrency;

            db.Add(operation);
            db.SaveChanges();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
