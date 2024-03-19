using AutoMapper;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using Contracts;
using Microsoft.AspNetCore.Authorization;

namespace AuctionService.Controller;

[ApiController]
[Route("api/auctions")]
public class AuctionController(IAuctionRepository repository, IMapper mapper, IPublishEndpoint publishEndpoint) : ControllerBase
{
    private readonly IAuctionRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
        => await _repository.GetAuctionAsync(date);

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _repository.GetAuctionByIdAsync(id);

        if (auction == null)
            return NotFound();
        return auction;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);

        // Todo: Add current user as seller
        auction.Seller = User.Identity.Name;

        _repository.AddAuction(auction);

        var newAuction = _mapper.Map<AuctionDto>(auction);

        //! Publish message to service bus
        await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

        var result = await _repository.SaveChangesAsync();

        if (!result) 
            return BadRequest("Could not save changes to the DB");

        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto auctionDto)
    {
        var auction = await _repository.GetAuctionEntityByIdAsync(id);

        if (auction == null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        Console.WriteLine($"Update model info: {auction.Item.Model} with Id: {auction.Id}");

        auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = auctionDto.Year ?? auction.Item.Year;

        //! Publish message to service bus
        await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _repository.SaveChangesAsync();

        if (!result) return BadRequest("Problem saving changes");

        return Ok();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _repository.GetAuctionEntityByIdAsync(id);

        if (auction == null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        _repository.RemoveAuction(auction);

        //! Publish message to service bus
        await _publishEndpoint.Publish(_mapper.Map<AuctionDeleted>(auction));

        var result = await _repository.SaveChangesAsync();

        if (!result) return BadRequest("Could not update DB");

        return Ok();
    }
}