using System;

namespace prog_part_2.Models
{
    public class Event
    {
        public int EventID { get; set; }
        public int OrganizerID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}