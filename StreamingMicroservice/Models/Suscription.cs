namespace StreamingMicroservice.Models
{
    public class Suscription
    {
        public int Id { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public int PaymentId { get; set; }

        public User? User { get; set; } 
        public Plan? Plan { get; set; } 
        public Payment? Payment { get; set; } 
    }
}
