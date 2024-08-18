using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Expense_Tracker.Models; // Adjust based on your namespace

public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context; // Adjust based on your context class

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Transactions/
    public async Task<IActionResult> Index()
    {
        DateTime StartDate = DateTime.Today.AddDays(-6);
        DateTime EndDate = DateTime.Today;

        var SelectedTransactions = await _context.Transactions
            .Include(x => x.Category)
            .Where(y => y.Date >= StartDate && y.Date <= EndDate)
            .ToListAsync();

        int TotalIncome = SelectedTransactions
            .Where(i => i.Category.Type == "Income")
            .Sum(i => i.Amount);

        int TotalExpense = SelectedTransactions
            .Where(i => i.Category.Type == "Expense")
            .Sum(i => i.Amount);

        int Balance = TotalIncome - TotalExpense;

        ViewBag.TotalIncome = TotalIncome.ToString("C0");
        ViewBag.TotalExpense = TotalExpense.ToString("C0");
        ViewBag.Balance = Balance.ToString("C0");

        // Generating Doughnut Chart Data - Expense By Category
        ViewBag.DoughnutChartData = SelectedTransactions
            .Where(i => i.Category.Type == "Expense")
            .GroupBy(j => j.Category.CategoryId)
            .Select(k => new
            {
                categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                amount = k.Sum(j => j.Amount),
                formattedAmount = k.Sum(j => j.Amount).ToString("C0")
            })
            .ToList();

        // Income Summary
        List<SplineChartData> IncomeSummary = SelectedTransactions
            .Where(i => i.Category.Type == "Income")
            .GroupBy(j => j.Date)
            .Select(k => new SplineChartData
            {
                Day = k.First().Date.ToString("dd-MMM"),
                Income = k.Sum(l => l.Amount)
            })
            .ToList();

        // Expense Summary
        List<SplineChartData> ExpenseSummary = SelectedTransactions
            .Where(i => i.Category.Type == "Expense")
            .GroupBy(j => j.Date)
            .Select(k => new SplineChartData
            {
                Day = k.First().Date.ToString("dd-MMM"),
                Expense = k.Sum(l => l.Amount)
            })
            .ToList();
        string[] Last7Days = Enumerable.Range(0, 7)
    .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
    .ToArray();
        var SplineChartData = from day in Last7Days
                              join income in IncomeSummary on day equals income.Day into dayIncomeJoined
                              from income in dayIncomeJoined.DefaultIfEmpty()
                              join expense in ExpenseSummary on day equals expense.Day into dayExpenseJoined
                              from expense in dayExpenseJoined.DefaultIfEmpty()
                              select new
                              {
                                  Day = day,
                                  Income = income?.Income ?? 0,  // If income is null, use 0
                                  Expense = expense?.Expense ?? 0  // If expense is null, use 0
                              };

        ViewBag.SplineChartData = SplineChartData.ToList();


        return View();
    }
    public class SplineChartData
    {
        public string Day { get; set; }
        public double Income { get; set; }
        public double Expense { get; set; }
    }

}
