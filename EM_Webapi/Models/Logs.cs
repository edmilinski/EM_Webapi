using System;

namespace EM_Webapi.Models
{
    public class Logs
    {
        public int Id { get; set; }
        public DateTime Modified { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
    }
}

