using System;
using System.Collections.Generic;
using System.Text;

namespace Phrasebook.Common
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
