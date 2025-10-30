using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Common.Interfaces;
using Application.Todos.Complete;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.AuctionBid.Create;

public class CreateAuctionBidCommandHandler(
    //IApplicationDbContext context,
    //IDateTimeProvider dateTimeProvider,
    //IUserContext userContext
)
    : ICommandHandler<CreateAuctionBidCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAuctionBidCommand command, CancellationToken cancellationToken)
    {

        return Result.Success(Guid.NewGuid());
    }
}
