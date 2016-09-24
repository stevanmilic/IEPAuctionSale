using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using IEPAuctionSale.Models;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.SignalR.Hubs;

namespace IEPAuctionSale
{
    [HubName("auctionHub")]
    public class AuctionHub : Hub
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void tickAuction(string connectionId, int auctionId)
        {
            Auction auction = db.Auctions.FirstOrDefault(a => a.Id == auctionId);
            TimeSpan difference = new TimeSpan(0);

            if (auction != null && auction.State == "OPEN")
            {
                DateTime auctionClosingTime = (DateTime)auction.TimeClosed;
                DateTime now = DateTime.Now;

                if (now >= auctionClosingTime)
                {
                    if (db.Bids.Any(b => b.AuctionId == auctionId))
                    {
                        auction.State = "SOLD";
                    }
                    else
                    {
                        auction.State = "EXPIRED";
                    }
                    try
                    {
                        db.Entry(auction).Property(a => a.State).IsModified = true;
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        db.SaveChanges();
                    }

                }
                else
                {
                    difference = auctionClosingTime.Subtract(now);
                }
            }
            Clients.Client(connectionId).timer(auction.Id, difference.ToString(@"dd\:hh\:mm\:ss"), auction.State);
        }

        public void Bid(string userId, int auctionId)
        {
            Auction auction = db.Auctions.FirstOrDefault(a => a.Id == auctionId && a.State == "OPEN");

            ApplicationUser user = db.Users.Find(userId);

            DateTime now = DateTime.Now;

            if (user.Tokens > 0 && DateTime.Now < (DateTime)auction.TimeClosed)
            {
                //set bid data
                Bid bid = new Bid();
                bid.AccountId = userId;
                bid.AuctionId = auctionId;
                bid.TimeBidded = now;

                //set auction data
                auction.CurrentBider = userId;

                int unitPrice = auction.StartingPrice == 0 ? 1 : (int)Math.Ceiling(auction.StartingPrice * 0.1);
                auction.CurrentPrice += unitPrice;

                TimeSpan timeRemaining = ((DateTime)auction.TimeClosed).Subtract(now);
                if (timeRemaining.TotalSeconds > 0 && timeRemaining.TotalSeconds < 10)
                {
                    auction.Duration += 10 - (int)timeRemaining.TotalSeconds;
                    auction.TimeClosed = ((DateTime)auction.TimeOpened).AddSeconds(auction.Duration);
                }

                //set user data
                user.Tokens--;

                try
                {
                    //update user tokens
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    //add new bid
                    db.Bids.Add(bid);

                    //row version is updated
                    db.SaveChanges();

                    //send data to clients
                    Clients.All.bid(auction.Id, user.Name + " " + user.Surname, auction.CurrentPrice);

                }
                catch (DbUpdateConcurrencyException e)
                {
                    //concurency error
                    log.Info("Mulitple users are bidding in the same time, concurency noticed!");
                }
            }
        }
    }
}