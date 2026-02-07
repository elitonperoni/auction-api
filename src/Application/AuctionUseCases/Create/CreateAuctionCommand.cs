using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;

namespace Application.AuctionUseCases.Create;

public class CreateAuctionCommand : ICommand<Guid>
{
    public Guid? Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime EndDate { get; set; }
    public decimal StartingPrice { get; set; }
    public List<FileInput> NewImages { get; set; }
    public List<string>? ImagesToRemove { get; set; }
}

public record FileInput(Stream Stream, string FileName, string ContentType);
