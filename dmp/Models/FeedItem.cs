namespace dmp.Models
{
    public class FeedItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public RssFeed RssFeed { get; set; }

        public FeedItem(string description, string url, string title, DateTime? date)
        {
            Description = description;
            Url = url;
            Title = title;
            Date = date;
        }
    }
}
