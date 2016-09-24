$(function () {

    var auction = $.connection.auctionHub;

    auction.client.timer = function (auctionId, currentTime, currentState) {
        document.getElementById("time" + auctionId).innerHTML = currentTime;
        document.getElementById("state" + auctionId).innerHTML = currentState;
    }

    auction.client.bid = function (auctionId, userFullName, updatedPrice) {
        document.getElementById("name" + auctionId).innerHTML = userFullName;
        document.getElementById("price" + auctionId).innerHTML = "$" + updatedPrice;
        $("<tr><td>" + userFullName + "</td><td>" + updatedPrice + "</td></tr>").insertAfter('table > tbody > tr:first');
        if (document.getElementById("bidtable").getElementsByTagName("tbody")[0].getElementsByTagName("tr").length > 10) {
            $("#bidtable tr:last").remove();
        }
    }

    $.connection.hub.start().done(function () {

        var intervalId;
        var auctions = document.getElementsByClassName("auction");

        auctionChecker = function () {
            i = auctions.length;
            if (i == 0) {
                clearInterval(intervalId);
            }
            while(i--){
                currentState = document.getElementById("state" + auctions[i].value).innerHTML;
                if (currentState === "OPEN") {
                    auctionModel = auction.server.tickAuction($.connection.hub.id, auctions[i].value);
                }
                else {
                    var bidAuctionButton = document.getElementById(auctions[i].value);
                    if(bidAuctionButton !== null)
                        bidAuctionButton.disabled = true;
                    auctions[i].parentNode.removeChild(auctions[i]);
                }
            }
        }

        intervalId = setInterval(auctionChecker, 500);
        
        $('.bidButton').click(function(event){
            var userId = document.getElementById("userId").value;
            var auctionId = event.target.id;
            auction.server.bid(userId, auctionId);
        });


    });
});