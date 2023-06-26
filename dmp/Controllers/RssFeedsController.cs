using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dmp;
using dmp.Models;
using CodeHollow.FeedReader;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using System.Web;
using System.Security.Permissions;
using System.Diagnostics;

namespace dmp.Controllers
{
    public class RssFeedsController : Controller
    {
        private readonly RssContext _context;

        public RssFeedsController(RssContext context)
        {
            _context = context;
        }

        // GET: RssFeeds
        public async Task<IActionResult> Index(string searchString)
        {
            if (searchString != null)
            {
                return _context.Feeds != null ?
                    View(await _context.Feeds.Where(x => x.Name.Contains(searchString)).ToListAsync()) :
                    Problem("Entity set 'RssContext.Feeds'  is null.");
            }

            return _context.Feeds != null ? 
                View(await _context.Feeds.ToListAsync()) :
                Problem("Entity set 'RssContext.Feeds'  is null.");
        }

        // GET: RssFeeds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Feeds == null)
            {
                return NotFound();
            }

            var rssFeed = await _context.Feeds.Include(x => x.Items)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rssFeed == null)
            {
                return NotFound();
            }

            if (HttpContext.Request.QueryString.HasValue)
            {
                NameValueCollection queryParameters = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.Value);
                if (DateTime.TryParse(queryParameters["From"], out DateTime fromDate) && 
                DateTime.TryParse(queryParameters["To"], out DateTime toDate))
                {
                    rssFeed.Items = rssFeed.Items.Where(x => x.Date > fromDate && x.Date < toDate).ToHashSet();
                }
            }

            return View(rssFeed);
        }

        // GET: RssFeeds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RssFeeds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Url")] RssFeed rssFeed)
        {
            try
            {
                var feed = await FeedReader.ReadAsync(rssFeed.Url);
                rssFeed.Title = feed.Title;
                rssFeed.Description = feed.Description;
                rssFeed.Items = new Collection<Models.FeedItem>();

                foreach (var item in feed.Items)
                {
                    Models.FeedItem feedItem = new Models.FeedItem(item.Description, item.Link, item.Title, item.PublishingDate);
                    rssFeed.Items.Add(feedItem);
                }

                _context.Add(rssFeed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
            
        }

        // GET: RssFeeds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Feeds == null)
            {
                return NotFound();
            }

            var rssFeed = await _context.Feeds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rssFeed == null)
            {
                return NotFound();
            }

            return View(rssFeed);
        }

        // POST: RssFeeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Feeds == null)
            {
                return Problem("Entity set 'RssContext.Feeds'  is null.");
            }
            var rssFeed = await _context.Feeds.FindAsync(id);
            if (rssFeed != null)
            {
                _context.Feeds.Remove(rssFeed);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reload(int? id)
        {
            if (id == null || _context.Feeds == null)
            {
                return NotFound();
            }

            var rssFeed = await _context.Feeds.Include(x => x.Items)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rssFeed == null)
            {
                return NotFound();
            }

            var feed = await FeedReader.ReadAsync(rssFeed.Url);
            rssFeed.Title = feed.Title;
            rssFeed.Description = feed.Description;
            rssFeed.Items = new Collection<Models.FeedItem>();

            foreach (var item in feed.Items)
            {
                Models.FeedItem feedItem = new Models.FeedItem(item.Description, item.Link, item.Title, item.PublishingDate);
                rssFeed.Items.Add(feedItem);
            }

            _context.Update(rssFeed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool RssFeedExists(int id)
        {
          return (_context.Feeds?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
