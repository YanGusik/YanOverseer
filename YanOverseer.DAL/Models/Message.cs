﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YanOverseer.DAL.Models
{
    public class Message
    {
        public ulong Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public ulong ProfileId { get; set; }
        public Profile Profile { get; set; }
    }
}
