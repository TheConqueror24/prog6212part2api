namespace prog_part_2.Models
{
    public class Enrolment
    {
        public int EnrolmentId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public DateTime EnrolmentDate { get; set; }
    }
}