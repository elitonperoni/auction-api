using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions;

public static class DateTimeExtension
{
    public static string GetCurrentTime()
    {
        string brasiliaTimeZoneId = "E. South America Standard Time";

        TimeZoneInfo brasiliaTimeZone;

        try
        {
            brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(brasiliaTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {            
            brasiliaTimeZoneId = "America/Sao_Paulo";

            brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(brasiliaTimeZoneId);
        }
        DateTime brasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brasiliaTimeZone);
        
        return brasiliaTime.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"));
    }
}
