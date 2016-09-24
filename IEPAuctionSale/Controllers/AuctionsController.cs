using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEPAuctionSale.Models;
using PagedList;
using Microsoft.AspNet.Identity;

namespace IEPAuctionSale.Controllers
{
    public class AuctionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Auctions
        public ActionResult Index(string searchStringProduct, int? minPrice, int? maxPrice, string searchState, string currentStringProduct, int? currentMinPrice, int? currentMaxPrice, string currentState, int? page)
        {
            IQueryable<Auction> auctions = Enumerable.Empty<Auction>().AsQueryable();

            searchStringProduct = searchStringProduct != null ? searchStringProduct : currentStringProduct;
            searchState = searchState != null ? searchState : currentState;
            minPrice = minPrice != null ? minPrice : currentMinPrice;
            maxPrice = maxPrice != null ? maxPrice : currentMaxPrice;

            ViewBag.CurrentSearch = searchStringProduct;
            ViewBag.CurrentMin = minPrice;
            ViewBag.CurrentMax = maxPrice;
            ViewBag.CurrentState = searchState;

            filterAuctions(ref auctions, searchStringProduct, minPrice, maxPrice, searchState);

            if (page == null && (!string.IsNullOrEmpty(searchStringProduct) || !string.IsNullOrEmpty(searchState) || minPrice != null || maxPrice != null))
            {
                page = 1;
            }
            else if(string.IsNullOrEmpty(currentStringProduct) && string.IsNullOrEmpty(currentState) && currentMinPrice == null && currentMaxPrice == null)
            {
                auctions = db.Auctions.Where(a => a.State == "OPEN").OrderByDescending(a => a.TimeOpened);
                auctions = auctions.Take(Math.Min(5, auctions.Count()));
            }


            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(auctions.Include(a => a.User).OrderByDescending(a => a.TimeCreated).ToPagedList(pageNumber, pageSize));
        }


        private void filterAuctions(ref IQueryable<Auction> auctions, string searchString, int? minPrice, int? maxPrice, string state)
        {
            if(User.IsInRole("Admin"))
            {
                auctions = db.Auctions.Where(a => a.State == "READY" || a.State == "OPEN" || a.State == "SOLD" || a.State == "EXPIRED");
            }
            else
            {
                auctions = db.Auctions.Where(a => a.State == "OPEN" || a.State == "SOLD" || a.State == "EXPIRED");
            }


            if (!string.IsNullOrEmpty(searchString))
            {
                string[] searchStrings = searchString.Split(' ');
                foreach (string s in searchStrings)
                {
                    auctions = auctions.Where(a => a.ProductName.Contains(s));
                }
            }

            if (!string.IsNullOrEmpty(state))
            {
                auctions = auctions.Where(a => a.State == state);
            }

            if(minPrice != null)
            {
                auctions = auctions.Where(a => a.CurrentPrice >= minPrice);
            }

            if(maxPrice != null && minPrice <= maxPrice)
            {
                auctions = auctions.Where(a => a.CurrentPrice <= maxPrice);
            }
        }

        [Authorize]
        public ActionResult ShowSoldAuctions(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            string user = User.Identity.GetUserId();
            IQueryable<Auction> auctions = db.Auctions.Where(a => a.State == "SOLD" && a.CurrentBider == user).OrderBy(a => a.TimeOpened);
            return View(auctions.ToPagedList(pageNumber, pageSize));

        }

        // GET: Auctions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // GET: Auctions/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductName,Duration,StartingPrice,ProductPictureUpload")] Auction auction)
        {
            if (ModelState.IsValid)
            { 
                auction.CurrentPrice = auction.StartingPrice;
                auction.TimeCreated = DateTime.Now;
                auction.State = "READY";
                if (auction.ProductPictureUpload != null)
                {
                    auction.ProductPicture = new Byte[auction.ProductPictureUpload.ContentLength];
                    auction.ProductPictureUpload.InputStream.Read(auction.ProductPicture, 0, auction.ProductPicture.Length);
                }
                db.Auctions.Add(auction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(auction);
        }

        // GET: Auctions/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductName,Duration,StartingPrice,RowVersion,TimeCreated,State,ProductPictureUpload")] Auction auction)
        {
            if (ModelState.IsValid)
            {
                auction.CurrentPrice = auction.StartingPrice;
                if(auction.ProductPictureUpload != null)
                {
                    auction.ProductPicture = new Byte[auction.ProductPictureUpload.ContentLength];
                    auction.ProductPictureUpload.InputStream.Read(auction.ProductPicture, 0, auction.ProductPicture.Length);
                }
                try
                {
                    db.Entry(auction).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (DBConcurrencyException)
                {
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(auction);
        }

        // GET: Auctions/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Auction auction = db.Auctions.Find(id);
            db.Auctions.Remove(auction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Open(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            if(auction.State != "READY")
            {
                return new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }
            auction.State = "OPEN";
            DateTime now = DateTime.Now;
            auction.TimeOpened = now;
            auction.TimeClosed = now.AddSeconds(auction.Duration);

            db.Entry(auction).Property(a => a.State).IsModified = true;
            db.Entry(auction).Property(a => a.TimeOpened).IsModified = true;
            db.Entry(auction).Property(a => a.TimeClosed).IsModified = true;
            db.SaveChanges();

            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
