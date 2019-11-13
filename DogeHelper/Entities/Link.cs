using System;
using System.Collections.Generic;
using System.Text;

namespace DogeHelper.Entities
{
    public class Link
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime LastChanged { get; set; }
        public string LastChangedBy { get; set; }
    }
}
