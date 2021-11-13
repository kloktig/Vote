namespace vote.Votes
{
    public record VoteCount
    {
        public string Name { get; init; }
        public int Count { get; init; }
        public decimal Percentage { get; init; }

    }
}