namespace flashplus.Models
{
    public class SearchModel
    {
        public string Search { get; set; }
        public string SearchType { get; set; }

        public string[] SearchTypes = { "Title", "Subject"};
        public string SortType { get; set; }

        public string[] SortTypes = { "AZ", "ZA"};
    }
}
