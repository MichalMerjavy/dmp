using dmp.Models;
using Microsoft.EntityFrameworkCore;

namespace dmp;

public class RssContext : DbContext
{
    public RssContext(DbContextOptions<RssContext> options) : base(options)
    {
        ;
    }

    public DbSet<RssFeed> Feeds { get; set; }
    public DbSet<FeedItem> FeedItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RssFeed>().ToTable("RssFeed");
        modelBuilder.Entity<FeedItem>().ToTable("FeedItem");
    }
}
