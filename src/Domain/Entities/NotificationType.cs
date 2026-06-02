using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities;

public sealed class NotificationType : Entity
{
    public int Id { get; set; }
    public string Name { get; set; }
}
