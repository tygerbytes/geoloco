using System;
using GeoLoco.Core.Interfaces;

namespace GeoLoco.Infrastructure.Services
{
    public class DateService : IDateService
    {
        public DateTime Now => DateTime.Now;
    }
}
