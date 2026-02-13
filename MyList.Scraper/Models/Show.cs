namespace MyList.Scraper.Models
{
    public class Show
    {
        public required DateOnly Date { get; set; }
        public TimeOnly? DoorsTime { get; set; }
        public List<TimeOnly>? StartTimes { get; set; }
        public TimeOnly? EndTime { get; set; }
        public required string Venue { get; set; }
        public required string City { get; set; }
        public required List<string> Artists { get; set; }
        public decimal? Price { get; set; }
        public bool Recommended { get; set; }
        public bool WillSellOut { get; set; }
        public bool U21DrinkTix { get; set; }
        public bool AllAges { get; set; }
        public string? AgeMeta { get; set; }
        public bool PitWarning { get; set; }
        public bool NoInsOuts { get; set; }
        public bool SoldOut { get; set; }

        public override string ToString()
        {
            string a = $"DATE: {Date}\nDOORS: {DoorsTime}\n";
            string b = $"START TIME(S): {(StartTimes != null ? string.Join(" ", StartTimes) : "")}\nEND TIME: {EndTime}\nVENUE: {Venue}\nCITY: {City}\n";
            string c = $"ARTISTS: {string.Join(", ", Artists)}\nPRICE: {Price}\nRECOMMENDED: {Recommended}\n";
            string d = $"WILL SELL OUT: {WillSellOut}\nUNDER 21 DRINK TIX: {U21DrinkTix}\nALL AGES: {AllAges}\n";
            string e = $"AGE META: {AgeMeta}\nPIT WARNING: {PitWarning}\nNO INS/OUTS: {NoInsOuts}\nSOLD OUT: {SoldOut}\n";
            return a + b + c + d + e;
        }
    }
}