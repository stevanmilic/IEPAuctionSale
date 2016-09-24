using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEPAuctionSale.Models;
using Microsoft.AspNet.Identity;

namespace IEPAuctionSale.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Orders
        [Authorize]
        public ActionResult Index()
        {
            String userId = User.Identity.GetUserId();
            return View(db.Orders.Where(o => o.AccountId == userId).OrderBy(o => o.TimeOrdered));
        }

        // GET: Orders/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        [HttpPost]
        [Authorize]
        public ActionResult OrderTokens()
        {
            Order order = new Order();
            order.AccountId = User.Identity.GetUserId();
            order.NoTokens = 0;
            order.PriceOfPackage = 0;
            order.TimeOrdered = DateTime.Now;
            order.StatusIndicatior = "PENDING";
            db.Orders.Add(order);
            db.SaveChanges();
            return Redirect("https://stage.centili.com/widget/WidgetModule?api=9bc0b4a5abf69691db7de7506b96f600&clientId=" + order.Id);
        }

        [HttpPost]
        public ActionResult OrderRealization(long clientid, string status, long amount, double enduserprice)
        {
            Order order = db.Orders.Find(clientid);
            if (order != null && order.StatusIndicatior == "PENDING")
            {
                if (status == "success")
                {
                    order.NoTokens = Convert.ToInt32(amount);
                    order.PriceOfPackage = enduserprice;
                    order.StatusIndicatior = "REALIZED";
                    ApplicationUser user = db.Users.Find(order.AccountId);
                    user.Tokens += Convert.ToInt32(amount);
                    db.Entry(user).State = EntityState.Modified;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    order.NoTokens = Convert.ToInt32(amount);
                    order.StatusIndicatior = "CANCELED";
                    order.PriceOfPackage = enduserprice;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            log.Info("External system(Centili) for paying is not working");
            return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,NoTokens,PriceOfPackage,TimeOrdered,StatusIndicatior")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
