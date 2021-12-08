using System;

namespace Phrasebook.Common
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
