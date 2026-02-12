namespace MyList.Scraper.Models
{
    public class Show
    {
        public DateOnly Date { get; set; }
        public TimeOnly? Time { get; set; }
        public string Venue { get; set; }
        public string City { get; set; }
        public List<string> Artists { get; set; }
        public decimal? Price { get; set; }
        public bool Recommended { get; set; }
        public bool WillSellOut { get; set; }
        public bool U21DrinkTix { get; set; }
        public bool AllAges { get; set; }
        public bool PitWarning { get; set; }
        public bool NoInsOuts { get; set; }
        public bool SoldOut { get; set; }
    }
}