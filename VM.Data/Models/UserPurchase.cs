#nullable disable
namespace VM.Data.Models
{
    public partial class UserPurchase
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string CurrencyIsoCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime PurchaseDate { get; set; }

        public virtual User User { get; set; }
    }
}