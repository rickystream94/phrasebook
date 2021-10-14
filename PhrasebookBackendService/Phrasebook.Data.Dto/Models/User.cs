﻿using System;
using System.Collections.Generic;

namespace Phrasebook.Data.Dto.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public DateTime SignedUpOn { get; set; }
    }
}