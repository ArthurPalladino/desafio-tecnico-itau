    public class ContributionHistory
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal NewValue { get; set; }
        public decimal OldValue { get; set; }
        public DateTime AlterationDate { get; set; }

        public virtual Customer Customer { get; set; }
    }