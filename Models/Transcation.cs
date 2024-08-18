using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int Amount { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [NotMapped]
        public string? CategoryWithIcon
        {
            get
            {
                return Category == null ? string.Empty : Category.Icon + " " + Category.Title;

            }
        }
        [NotMapped]
        public string? FormatedAmount
        {
            get
            {
                return (Category == null || Category.Type == "Expense" ? "-" : "+") + Amount.ToString("C0");

            }
        }
    }
}