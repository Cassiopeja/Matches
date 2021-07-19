using System;

namespace Pexeso.Services
{
    public interface IDateTimeService
    {
        DateTimeOffset Now { get; }
    }
}