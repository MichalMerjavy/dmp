namespace dmp.Models
{
    public class RssFeed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public ICollection<FeedItem> Items { get; set; }

        public RssFeed(string name, string description, string url, string title)
        {
            Name = name;
            Description = description;
            Url = url;
            Title = title;
            Items = new List<FeedItem>();
        }

        public RssFeed()
        {
        }
    }
}
