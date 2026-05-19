using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Mail;

public record SendEmailCommand(string Subject, string Body, string To);
