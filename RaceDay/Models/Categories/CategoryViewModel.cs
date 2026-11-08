namespace RaceDay.Models.Categories
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Distance { get; set; }        // decimal, because your SQL column is DECIMAL
        public decimal EntryFee { get; set; }
        public int MaxParticipants { get; set; }
    }
}
