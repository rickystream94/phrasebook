using System;

namespace Phrasebook.Common
{
    public interface ITimeProvider
    {
        public DateTime Now { get; }
    }
}
