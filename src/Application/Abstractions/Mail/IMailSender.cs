using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Mail;
using AuctionApi.Common;
using Microsoft.Extensions.Options;

namespace Application.Abstractions.Mail;

public interface IMailSender
{
    Task SendEmail(SendEmailCommand requestEmail, IOptions<SecretsApi> options);
}
