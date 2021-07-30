using System;

namespace Matches.Services
{
    public interface IDateTimeService
    {
        DateTimeOffset Now { get; }
    }
}