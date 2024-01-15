using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class AuctionUpdatedFaultConsumer : IConsumer<Fault<AuctionUpdated>>
{
    public Task Consume(ConsumeContext<Fault<AuctionUpdated>> context)
    {
        Console.WriteLine("--> Update dashboard something to visualize that");
        
        return null;
    }
}
