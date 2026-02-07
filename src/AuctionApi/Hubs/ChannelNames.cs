using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionApi.Hubs;

public static class ChannelNames
{
    public const string ReceiveNewBid = "ReceiveNewBid";
    public const string SendBid = "SendBid";
    public const string ReceiveNotification = "ReceiveNotification";
}
