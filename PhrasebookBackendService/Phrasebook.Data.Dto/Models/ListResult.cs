using System;
using System.Collections.Generic;

namespace Phrasebook.Data.Dto.Models
{
    public class ListResult<T>
    {
        public ListResult(T[] array)
        {
            this.Value = array ?? Array.Empty<T>();
        }

        public IReadOnlyCollection<T> Value { get; set; }
    }
}
