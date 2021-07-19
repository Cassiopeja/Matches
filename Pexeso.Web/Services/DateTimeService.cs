using System;

namespace Pexeso.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}