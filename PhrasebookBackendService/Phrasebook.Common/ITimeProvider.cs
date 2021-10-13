using System;
using System.Collections.Generic;
using System.Text;

namespace Phrasebook.Common
{
    public interface ITimeProvider
    {
        public DateTime Now { get; }
    }
}
