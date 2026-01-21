namespace GymSystem.Models
{
    public class MemberWeight
    {
        public int Id { get; set; }
        
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public double Weight { get; set; }
        
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
