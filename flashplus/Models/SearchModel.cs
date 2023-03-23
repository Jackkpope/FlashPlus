namespace flashplus.Models
{
    public class SearchModel
    {
        public string Search { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public bool OrderByAlphabetical { get; set; }
        public bool OrderByAlphabeticalInverse { get; set; }
        public bool OrderByAge { get; set; }
    }
}
