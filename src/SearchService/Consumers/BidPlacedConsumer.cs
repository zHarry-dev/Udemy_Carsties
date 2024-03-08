﻿using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("--> Consuming bid placed");

        var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

        if (context.Message.BidStatus.Contains("Accepted") && context.Message.Amount > (auction.CurrentHighBid ?? 0))
        {
            auction.CurrentHighBid = context.Message.Amount;
            await auction.SaveAsync();

            Console.WriteLine("--> Consuming bid placed complete!");
        }
    }
}